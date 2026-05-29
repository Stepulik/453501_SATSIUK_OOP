from fastapi import APIRouter, Depends, HTTPException, Query
from sqlalchemy.orm import Session
from datetime import datetime
from typing import Optional, List
from clients.database import get_db
from clients.repositories.transaction_repo import TransactionRepository
from clients.repositories.account_repo import AccountRepository
from clients.repositories.category_repo import CategoryRepository
from clients.repositories.budget_repo import BudgetRepository
from controllers.transaction_controller import TransactionController
from models.request_models import TransactionCreateRequest
from models.response_models import TransactionResponse, ErrorResponse
from api.dependencies import get_current_user
from models.domain_models import UserDomain
from shared.errors import NotFoundError, DomainError

router = APIRouter()

def get_transaction_controller(db: Session = Depends(get_db)) -> TransactionController:
    transaction_repo = TransactionRepository(db)
    account_repo = AccountRepository(db)
    category_repo = CategoryRepository(db)
    budget_repo = BudgetRepository(db)
    return TransactionController(transaction_repo, account_repo, category_repo, budget_repo)

@router.post("/", response_model=TransactionResponse, responses={400: {"model": ErrorResponse}, 404: {"model": ErrorResponse}})
def create_transaction(
    req: TransactionCreateRequest,
    current_user: UserDomain = Depends(get_current_user),
    controller: TransactionController = Depends(get_transaction_controller)
):
    try:
        transaction = controller.create_transaction(current_user.id, req)
    except NotFoundError as e:
        raise HTTPException(status_code=404, detail=str(e))
    except DomainError as e:
        raise HTTPException(status_code=400, detail=str(e))
    return TransactionResponse(
        id=transaction.id,
        amount=transaction.amount,
        description=transaction.description,
        date=transaction.date,
        from_account_id=transaction.from_account_id,
        to_account_id=transaction.to_account_id,
        category_id=transaction.category_id,
        user_id=transaction.user_id
    )

@router.get("/", response_model=list[TransactionResponse])
def get_transactions(
    start_date: Optional[datetime] = Query(None),
    end_date: Optional[datetime] = Query(None),
    current_user: UserDomain = Depends(get_current_user),
    controller: TransactionController = Depends(get_transaction_controller)
):
    transactions = controller.get_user_transactions(current_user.id, start_date, end_date)
    return [
        TransactionResponse(
            id=t.id,
            amount=t.amount,
            description=t.description,
            date=t.date,
            from_account_id=t.from_account_id,
            to_account_id=t.to_account_id,
            category_id=t.category_id,
            user_id=t.user_id
        ) for t in transactions
    ]

@router.get("/{transaction_id}", response_model=TransactionResponse, responses={404: {"model": ErrorResponse}})
def get_transaction(
    transaction_id: int,
    current_user: UserDomain = Depends(get_current_user),
    controller: TransactionController = Depends(get_transaction_controller)
):
    try:
        transaction = controller.get_transaction(transaction_id, current_user.id)
    except NotFoundError as e:
        raise HTTPException(status_code=404, detail=str(e))
    return TransactionResponse(
        id=transaction.id,
        amount=transaction.amount,
        description=transaction.description,
        date=transaction.date,
        from_account_id=transaction.from_account_id,
        to_account_id=transaction.to_account_id,
        category_id=transaction.category_id,
        user_id=transaction.user_id
    )


@router.delete("/{transaction_id}", status_code=204, responses={404: {"model": ErrorResponse}})
def delete_transaction(
    transaction_id: int,
    current_user: UserDomain = Depends(get_current_user),
    controller: TransactionController = Depends(get_transaction_controller)
):
    try:
        controller.delete_transaction(transaction_id, current_user.id)
    except NotFoundError as e:
        raise HTTPException(status_code=404, detail=str(e))
    return
