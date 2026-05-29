from typing import List
from models.domain_models import AccountDomain
from models.request_models import AccountCreateRequest
from controllers.account_repository_interface import IAccountRepository
from shared.errors import NotFoundError

class AccountController:
    def __init__(self, account_repo: IAccountRepository):
        self._account_repo = account_repo

    def create_account(self, user_id: int, req: AccountCreateRequest) -> AccountDomain:
        account = AccountDomain(
            id=None,
            name=req.name,
            balance=req.initial_balance,
            user_id=user_id
        )
        return self._account_repo.create(account)

    def get_user_accounts(self, user_id: int) -> List[AccountDomain]:
        return self._account_repo.get_by_user(user_id)

    def get_account(self, account_id: int, user_id: int) -> AccountDomain:
        account = self._account_repo.get_by_id(account_id, user_id)
        if not account:
            raise NotFoundError(f"Account {account_id} not found")
        return account

    def delete_account(self, account_id: int, user_id: int) -> None:
        self._account_repo.delete(account_id, user_id)
