from typing import List
from models.domain_models import BudgetDomain
from models.request_models import BudgetCreateRequest
from controllers.budget_repository_interface import IBudgetRepository
from controllers.category_repository_interface import ICategoryRepository
from shared.errors import AlreadyExistsError, NotFoundError

class BudgetController:
    def __init__(self, budget_repo: IBudgetRepository, category_repo: ICategoryRepository):
        self._budget_repo = budget_repo
        self._category_repo = category_repo

    def create_budget(self, user_id: int, req: BudgetCreateRequest) -> BudgetDomain:
        # Проверяем существование категории
        category = self._category_repo.get_by_id(req.category_id, user_id)
        if not category:
            raise NotFoundError(f"Category {req.category_id} not found")
        # Проверяем, нет ли уже бюджета на эту категорию и месяц
        existing = self._budget_repo.get_by_category_and_month(req.category_id, user_id, req.month, req.year)
        if existing:
            raise AlreadyExistsError(f"Budget for category {req.category_id} in {req.month}/{req.year} already exists")
        budget = BudgetDomain(
            id=None,
            category_id=req.category_id,
            user_id=user_id,
            month=req.month,
            year=req.year,
            limit_amount=req.limit_amount
        )
        return self._budget_repo.create(budget)

    def get_user_budgets(self, user_id: int) -> List[BudgetDomain]:
        return self._budget_repo.get_by_user(user_id)

    def get_budget(self, budget_id: int, user_id: int) -> BudgetDomain:
        budget = self._budget_repo.get_by_id(budget_id, user_id)
        if not budget:
            raise NotFoundError(f"Budget {budget_id} not found")
        return budget

    def delete_budget(self, budget_id: int, user_id: int) -> None:
        self.get_budget(budget_id, user_id)
        self._budget_repo.delete(budget_id, user_id)
