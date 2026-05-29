from abc import ABC, abstractmethod
from typing import Optional
from models.domain_models import UserDomain

class IUserRepository(ABC):
    @abstractmethod
    async def create(self, user: UserDomain) -> UserDomain:
        pass

    @abstractmethod
    async def get_by_username(self, username: str) -> Optional[UserDomain]:
        pass

    @abstractmethod
    async def get_by_email(self, email: str) -> Optional[UserDomain]:
        pass

    @abstractmethod
    async def get_by_id(self, user_id: int) -> Optional[UserDomain]:
        pass
