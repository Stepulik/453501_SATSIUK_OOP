from abc import ABC, abstractmethod
from typing import List, Optional
from datetime import datetime
from models.domain_models import TransactionDomain

class ITransactionRepository(ABC):
    @abstractmethod
    def create(self, transaction: TransactionDomain) -> TransactionDomain:
        pass

    @abstractmethod
    def get_by_id(self, transaction_id: int, user_id: int) -> Optional[TransactionDomain]:
        pass

    @abstractmethod
    def get_by_user(self, user_id: int, start_date: Optional[datetime] = None, end_date: Optional[datetime] = None) -> List[TransactionDomain]:
        pass

    @abstractmethod
    def delete(self, transaction_id: int, user_id: int) -> None:
        pass

    @abstractmethod
    def get_sum_by_category_and_month(self, category_id: int, user_id: int, month: int, year: int) -> float:
        pass
