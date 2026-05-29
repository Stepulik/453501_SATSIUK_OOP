from abc import ABC, abstractmethod
from typing import List, Optional
from models.domain_models import AccountDomain

class IAccountRepository(ABC):
    @abstractmethod
    def create(self, account: AccountDomain) -> AccountDomain:
        pass

    @abstractmethod
    def get_by_id(self, account_id: int, user_id: int) -> Optional[AccountDomain]:
        pass

    @abstractmethod
    def get_by_user(self, user_id: int) -> List[AccountDomain]:
        pass

    @abstractmethod
    def update_balance(self, account_id: int, new_balance: float) -> None:
        pass

    @abstractmethod
    def delete(self, account_id: int, user_id: int) -> None:
        pass
