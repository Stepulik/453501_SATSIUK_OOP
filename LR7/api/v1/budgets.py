from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from clients.database import get_db
from clients.repositories.budget_repo import BudgetRepository
from clients.repositories.category_repo import CategoryRepository
from controllers.budget_controller import BudgetController
from models.request_models import BudgetCreateRequest
from models.response_models import BudgetResponse, ErrorResponse
from api.dependencies import get_current_user
from models.domain_models import UserDomain
from shared.errors import AlreadyExistsError, NotFoundError

router = APIRouter()

def get_budget_controller(db: Session = Depends(get_db)) -> BudgetController:
    budget_repo = BudgetRepository(db)
    category_repo = CategoryRepository(db)
    return BudgetController(budget_repo, category_repo)

@router.post("/", response_model=BudgetResponse, responses={400: {"model": ErrorResponse}, 404: {"model": ErrorResponse}})
def create_budget(
    req: BudgetCreateRequest,
    current_user: UserDomain = Depends(get_current_user),
    controller: BudgetController = Depends(get_budget_controller)
):
    try:
        budget = controller.create_budget(current_user.id, req)
    except NotFoundError as e:
        raise HTTPException(status_code=404, detail=str(e))
    except AlreadyExistsError as e:
        raise HTTPException(status_code=400, detail=str(e))
    return BudgetResponse(
        id=budget.id,
        category_id=budget.category_id,
        user_id=budget.user_id,
        month=budget.month,
        year=budget.year,
        limit_amount=budget.limit_amount,
        created_at=budget.created_at,
        updated_at=budget.updated_at
    )

@router.get("/", response_model=list[BudgetResponse])
def get_budgets(
    current_user: UserDomain = Depends(get_current_user),
    controller: BudgetController = Depends(get_budget_controller)
):
    budgets = controller.get_user_budgets(current_user.id)
    return [
        BudgetResponse(
            id=b.id,
            category_id=b.category_id,
            user_id=b.user_id,
            month=b.month,
            year=b.year,
            limit_amount=b.limit_amount,
            created_at=b.created_at,
            updated_at=b.updated_at
        ) for b in budgets
    ]

@router.delete("/{budget_id}", status_code=204, responses={404: {"model": ErrorResponse}})
def delete_budget(
    budget_id: int,
    current_user: UserDomain = Depends(get_current_user),
    controller: BudgetController = Depends(get_budget_controller)
):
    try:
        controller.delete_budget(budget_id, current_user.id)
    except NotFoundError as e:
        raise HTTPException(status_code=404, detail=str(e))
    return
