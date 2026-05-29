from sqlalchemy.orm import Session
from clients.orm_models import User
from models.domain_models import UserDomain
from controllers.user_repository_interface import IUserRepository
from typing import Optional

class UserRepository(IUserRepository):
    def __init__(self, session: Session):
        self._session = session

    def create(self, user: UserDomain) -> UserDomain:
        db_user = User(
            email=user.email,
            username=user.username,
            hashed_password=user.hashed_password
        )
        self._session.add(db_user)
        self._session.commit()
        self._session.refresh(db_user)
        return UserDomain(
            id=db_user.id,
            email=db_user.email,
            username=db_user.username,
            hashed_password=db_user.hashed_password,
            created_at=db_user.created_at,
            updated_at=db_user.updated_at
        )

    def get_by_username(self, username: str) -> Optional[UserDomain]:
        db_user = self._session.query(User).filter(User.username == username).first()
        if db_user:
            return UserDomain(
                id=db_user.id,
                email=db_user.email,
                username=db_user.username,
                hashed_password=db_user.hashed_password,
                created_at=db_user.created_at,
                updated_at=db_user.updated_at
            )
        return None

    def get_by_email(self, email: str) -> Optional[UserDomain]:
        db_user = self._session.query(User).filter(User.email == email).first()
        if db_user:
            return UserDomain(
                id=db_user.id,
                email=db_user.email,
                username=db_user.username,
                hashed_password=db_user.hashed_password,
                created_at=db_user.created_at,
                updated_at=db_user.updated_at
            )
        return None

    def get_by_id(self, user_id: int) -> Optional[UserDomain]:
        db_user = self._session.query(User).filter(User.id == user_id).first()
        if db_user:
            return UserDomain(
                id=db_user.id,
                email=db_user.email,
                username=db_user.username,
                hashed_password=db_user.hashed_password,
                created_at=db_user.created_at,
                updated_at=db_user.updated_at
            )
        return None
