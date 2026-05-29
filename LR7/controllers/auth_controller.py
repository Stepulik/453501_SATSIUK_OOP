from models.domain_models import UserDomain
from models.request_models import UserRegisterRequest, UserLoginRequest
from controllers.user_repository_interface import IUserRepository
from shared.security import get_password_hash, verify_password, create_access_token
from shared.errors import AlreadyExistsError, UnauthorizedError

class AuthController:
    def __init__(self, user_repo: IUserRepository):
        self._user_repo = user_repo

    def register(self, req: UserRegisterRequest) -> UserDomain:
        existing = self._user_repo.get_by_username(req.username)
        if existing:
            raise AlreadyExistsError(f"User {req.username} already exists")
        existing_email = self._user_repo.get_by_email(req.email)
        if existing_email:
            raise AlreadyExistsError(f"Email {req.email} already registered")

        hashed = get_password_hash(req.password)
        new_user = UserDomain(
            id=None,
            email=req.email,
            username=req.username,
            hashed_password=hashed
        )
        return self._user_repo.create(new_user)

    def login(self, req: UserLoginRequest) -> str:
        user = self._user_repo.get_by_username(req.username)
        if not user:
            raise UnauthorizedError("Invalid credentials")
        if not verify_password(req.password, user.hashed_password):
            raise UnauthorizedError("Invalid credentials")
        access_token = create_access_token(data={"sub": user.username})
        return access_token
