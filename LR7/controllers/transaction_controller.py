from typing import List, Optional
from datetime import datetime
from models.domain_models import TransactionDomain
from models.request_models import TransactionCreateRequest
from controllers.transaction_repository_interface import ITransactionRepository
from controllers.account_repository_interface import IAccountRepository
from controllers.category_repository_interface import ICategoryRepository
from controllers.budget_repository_interface import IBudgetRepository
from shared.errors import NotFoundError, DomainError

class TransactionController:
    def __init__(
        self,
        transaction_repo: ITransactionRepository,
        account_repo: IAccountRepository,
        category_repo: ICategoryRepository,
        budget_repo: IBudgetRepository
    ):
        self._transaction_repo = transaction_repo
        self._account_repo = account_repo
        self._category_repo = category_repo
        self._budget_repo = budget_repo

    def _check_budget(self, user_id: int, category_id: int, amount: float, transaction_date: datetime):
        # Получаем бюджет на месяц транзакции
        month = transaction_date.month
        year = transaction_date.year
        budget = self._budget_repo.get_by_category_and_month(category_id, user_id, month, year)
        if budget is None:
            return  # нет бюджета — не проверяем
        # Суммируем все расходы по этой категории за месяц (включая текущую)
        from controllers.transaction_repository_interface import ITransactionRepository
        # Чтобы не было циклической зависимости, можно передать репозиторий, но проще вычислить здесь через прямой запрос? Лучше добавить метод в репозиторий.
        # Упростим: сделаем метод в transaction_repo для суммы расходов по категории за месяц.
        spent = self._transaction_repo.get_sum_by_category_and_month(category_id, user_id, month, year)
        if spent + amount > budget.limit_amount:
            raise DomainError(f"Budget limit exceeded for category {category_id} in {month}/{year}. Limit: {budget.limit_amount}, spent: {spent}, attempted: {amount}")

    def create_transaction(self, user_id: int, req: TransactionCreateRequest) -> TransactionDomain:
        from_account = self._account_repo.get_by_id(req.from_account_id, user_id)
        if not from_account:
            raise NotFoundError(f"Account {req.from_account_id} not found")

        # Для расходов проверим бюджет
        if req.to_account_id is None and req.category_id is not None:
            category = self._category_repo.get_by_id(req.category_id, user_id)
            if category and category.type == "expense":
                self._check_budget(user_id, req.category_id, req.amount, req.date or datetime.now())


        # Обработка переводов, доходов, расходов (как раньше)
        if req.to_account_id is not None:
            to_account = self._account_repo.get_by_id(req.to_account_id, user_id)
            if not to_account:
                raise NotFoundError(f"Account {req.to_account_id} not found")
            if from_account.balance < req.amount:
                raise DomainError("Insufficient funds on source account")
            self._account_repo.update_balance(from_account.id, from_account.balance - req.amount)
            self._account_repo.update_balance(to_account.id, to_account.balance + req.amount)
        else:
            if req.amount <= 0:
                raise DomainError("Amount must be positive")
            if req.category_id is not None:
                category = self._category_repo.get_by_id(req.category_id, user_id)
                if not category:
                    raise NotFoundError(f"Category {req.category_id} not found")
                if category.type == "income":
                    self._account_repo.update_balance(from_account.id, from_account.balance + req.amount)
                elif category.type == "expense":
                    if from_account.balance < req.amount:
                        raise DomainError("Insufficient funds")
                    self._account_repo.update_balance(from_account.id, from_account.balance - req.amount)
                else:
                    raise DomainError("Invalid category type")
            else:
                # без категории считаем расходом
                if from_account.balance < req.amount:
                    raise DomainError("Insufficient funds")
                self._account_repo.update_balance(from_account.id, from_account.balance - req.amount)

        transaction = TransactionDomain(
            id=None,
            amount=req.amount,
            description=req.description,
            date=req.date or datetime.now(),
            from_account_id=req.from_account_id,
            to_account_id=req.to_account_id,
            category_id=req.category_id,
            user_id=user_id
        )
        return self._transaction_repo.create(transaction)

    def get_user_transactions(self, user_id: int, start_date: Optional[datetime] = None, end_date: Optional[datetime] = None) -> List[TransactionDomain]:
        return self._transaction_repo.get_by_user(user_id, start_date, end_date)

    def get_transaction(self, transaction_id: int, user_id: int) -> TransactionDomain:
        transaction = self._transaction_repo.get_by_id(transaction_id, user_id)
        if not transaction:
            raise NotFoundError(f"Transaction {transaction_id} not found")
        return transaction

    def delete_transaction(self, transaction_id: int, user_id: int) -> None:
        self.get_transaction(transaction_id, user_id)
        self._transaction_repo.delete(transaction_id, user_id)
