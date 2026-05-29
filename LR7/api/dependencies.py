from fastapi import Depends, HTTPException
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from jose import JWTError, jwt
from sqlalchemy.orm import Session
from clients.database import get_db
from clients.repositories.user_repo import UserRepository
from models.domain_models import UserDomain
from shared.config import settings
from shared.errors import UnauthorizedError

security = HTTPBearer()

def get_current_user(credentials: HTTPAuthorizationCredentials = Depends(security), db: Session = Depends(get_db)) -> UserDomain:
    token = credentials.credentials
    try:
        payload = jwt.decode(token, settings.SECRET_KEY, algorithms=[settings.ALGORITHM])
        username: str = payload.get("sub")
        if username is None:
            raise UnauthorizedError("Invalid token")
    except JWTError:
        raise UnauthorizedError("Invalid token")
    user_repo = UserRepository(db)
    user = user_repo.get_by_username(username)
    if user is None:
        raise UnauthorizedError("User not found")
    return user
