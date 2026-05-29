from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from clients.database import get_db
from clients.repositories.user_repo import UserRepository
from controllers.auth_controller import AuthController
from models.request_models import UserRegisterRequest, UserLoginRequest
from models.response_models import TokenResponse, UserResponse, ErrorResponse
from shared.errors import AlreadyExistsError, UnauthorizedError

router = APIRouter()

def get_auth_controller(db: Session = Depends(get_db)) -> AuthController:
    user_repo = UserRepository(db)
    return AuthController(user_repo)

@router.post("/register", response_model=UserResponse, responses={400: {"model": ErrorResponse}})
def register(req: UserRegisterRequest, controller: AuthController = Depends(get_auth_controller)):
    try:
        user = controller.register(req)
        return UserResponse(id=user.id, email=user.email, username=user.username)
    except AlreadyExistsError as e:
        raise HTTPException(status_code=400, detail=str(e))

@router.post("/login", response_model=TokenResponse, responses={401: {"model": ErrorResponse}})
def login(req: UserLoginRequest, controller: AuthController = Depends(get_auth_controller)):
    try:
        token = controller.login(req)
        return TokenResponse(access_token=token)
    except UnauthorizedError as e:
        raise HTTPException(status_code=401, detail=str(e))
