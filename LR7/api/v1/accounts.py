from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from clients.database import get_db
from clients.repositories.account_repo import AccountRepository
from controllers.account_controller import AccountController
from models.request_models import AccountCreateRequest
from models.response_models import AccountResponse, ErrorResponse
from api.dependencies import get_current_user
from models.domain_models import UserDomain
from shared.errors import NotFoundError

router = APIRouter()

def get_account_controller(db: Session = Depends(get_db)) -> AccountController:
    repo = AccountRepository(db)
    return AccountController(repo)

@router.post("/", response_model=AccountResponse, responses={400: {"model": ErrorResponse}})
def create_account(
    req: AccountCreateRequest,
    current_user: UserDomain = Depends(get_current_user),
    controller: AccountController = Depends(get_account_controller)
):
    account = controller.create_account(current_user.id, req)
    return AccountResponse(
        id=account.id,
        name=account.name,
        balance=account.balance,
        user_id=account.user_id,
        created_at=account.created_at,
        updated_at=account.updated_at
    )

@router.get("/", response_model=list[AccountResponse])
def get_accounts(
    current_user: UserDomain = Depends(get_current_user),
    controller: AccountController = Depends(get_account_controller)
):
    accounts = controller.get_user_accounts(current_user.id)
    return [
        AccountResponse(
            id=a.id,
            name=a.name,
            balance=a.balance,
            user_id=a.user_id,
            created_at=a.created_at,
            updated_at=a.updated_at
        ) for a in accounts
    ]

@router.get("/{account_id}", response_model=AccountResponse, responses={404: {"model": ErrorResponse}})
def get_account(
    account_id: int,
    current_user: UserDomain = Depends(get_current_user),
    controller: AccountController = Depends(get_account_controller)
):
    try:
        account = controller.get_account(account_id, current_user.id)
    except NotFoundError as e:
        raise HTTPException(status_code=404, detail=str(e))
    return AccountResponse(
        id=account.id,
        name=account.name,
        balance=account.balance,
        user_id=account.user_id,
        created_at=account.created_at,
        updated_at=account.updated_at
    )

@router.delete("/{account_id}", status_code=204, responses={404: {"model": ErrorResponse}})
def delete_account(
    account_id: int,
    current_user: UserDomain = Depends(get_current_user),
    controller: AccountController = Depends(get_account_controller)
):
    try:
        controller.delete_account(account_id, current_user.id)
    except NotFoundError as e:
        raise HTTPException(status_code=404, detail=str(e))
    return
