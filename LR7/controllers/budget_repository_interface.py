from abc import ABC, abstractmethod
from typing import List, Optional
from models.domain_models import BudgetDomain

class IBudgetRepository(ABC):
    @abstractmethod
    def create(self, budget: BudgetDomain) -> BudgetDomain:
        pass

    @abstractmethod
    def get_by_id(self, budget_id: int, user_id: int) -> Optional[BudgetDomain]:
        pass

    @abstractmethod
    def get_by_user(self, user_id: int) -> List[BudgetDomain]:
        pass

    @abstractmethod
    def get_by_category_and_month(self, category_id: int, user_id: int, month: int, year: int) -> Optional[BudgetDomain]:
        pass

    @abstractmethod
    def delete(self, budget_id: int, user_id: int) -> None:
        pass
