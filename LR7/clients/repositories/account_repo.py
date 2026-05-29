from sqlalchemy.orm import Session
from typing import List, Optional
from clients.orm_models import Account
from models.domain_models import AccountDomain
from controllers.account_repository_interface import IAccountRepository

class AccountRepository(IAccountRepository):
    def __init__(self, session: Session):
        self._session = session

    def create(self, account: AccountDomain) -> AccountDomain:
        db_account = Account(
            name=account.name,
            balance=account.balance,
            user_id=account.user_id
        )
        self._session.add(db_account)
        self._session.commit()
        self._session.refresh(db_account)
        return self._to_domain(db_account)

    def get_by_id(self, account_id: int, user_id: int) -> Optional[AccountDomain]:
        db_account = self._session.query(Account).filter(Account.id == account_id, Account.user_id == user_id).first()
        return self._to_domain(db_account) if db_account else None

    def get_by_user(self, user_id: int) -> List[AccountDomain]:
        db_accounts = self._session.query(Account).filter(Account.user_id == user_id).all()
        return [self._to_domain(a) for a in db_accounts]

    def update_balance(self, account_id: int, new_balance: float) -> None:
        self._session.query(Account).filter(Account.id == account_id).update({"balance": new_balance})
        self._session.commit()

    def delete(self, account_id: int, user_id: int) -> None:
        self._session.query(Account).filter(Account.id == account_id, Account.user_id == user_id).delete()
        self._session.commit()

    def _to_domain(self, db_account: Account) -> AccountDomain:
        return AccountDomain(
            id=db_account.id,
            name=db_account.name,
            balance=db_account.balance,
            user_id=db_account.user_id,
            created_at=db_account.created_at,
            updated_at=db_account.updated_at
        )
