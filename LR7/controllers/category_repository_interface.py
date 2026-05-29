from abc import ABC, abstractmethod
from typing import List, Optional
from models.domain_models import CategoryDomain

class ICategoryRepository(ABC):
    @abstractmethod
    def create(self, category: CategoryDomain) -> CategoryDomain:
        pass

    @abstractmethod
    def get_by_id(self, category_id: int, user_id: int) -> Optional[CategoryDomain]:
        pass

    @abstractmethod
    def get_by_user(self, user_id: int) -> List[CategoryDomain]:
        pass

    @abstractmethod
    def get_by_name_and_type(self, user_id: int, name: str, type: str) -> Optional[CategoryDomain]:
        pass

    @abstractmethod
    def delete(self, category_id: int, user_id: int) -> None:
        pass
