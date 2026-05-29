from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from clients.database import get_db
from clients.repositories.category_repo import CategoryRepository
from controllers.category_controller import CategoryController
from models.request_models import CategoryCreateRequest
from models.response_models import CategoryResponse, ErrorResponse
from api.dependencies import get_current_user
from models.domain_models import UserDomain
from shared.errors import AlreadyExistsError, NotFoundError

router = APIRouter()

def get_category_controller(db: Session = Depends(get_db)) -> CategoryController:
    repo = CategoryRepository(db)
    return CategoryController(repo)

@router.post("/", response_model=CategoryResponse, responses={400: {"model": ErrorResponse}})
def create_category(
    req: CategoryCreateRequest,
    current_user: UserDomain = Depends(get_current_user),
    controller: CategoryController = Depends(get_category_controller)
):
    try:
        category = controller.create_category(current_user.id, req)
    except AlreadyExistsError as e:
        raise HTTPException(status_code=400, detail=str(e))
    return CategoryResponse(
        id=category.id,
        name=category.name,
        type=category.type,
        user_id=category.user_id,
        created_at=category.created_at
    )

@router.get("/", response_model=list[CategoryResponse])
def get_categories(
    current_user: UserDomain = Depends(get_current_user),
    controller: CategoryController = Depends(get_category_controller)
):
    categories = controller.get_user_categories(current_user.id)
    return [
        CategoryResponse(
            id=c.id,
            name=c.name,
            type=c.type,
            user_id=c.user_id,
            created_at=c.created_at
        ) for c in categories
    ]

@router.get("/{category_id}", response_model=CategoryResponse, responses={404: {"model": ErrorResponse}})
def get_category(
    category_id: int,
    current_user: UserDomain = Depends(get_current_user),
    controller: CategoryController = Depends(get_category_controller)
):
    try:
        category = controller.get_category(category_id, current_user.id)
    except NotFoundError as e:
        raise HTTPException(status_code=404, detail=str(e))
    return CategoryResponse(
        id=category.id,
        name=category.name,
        type=category.type,
        user_id=category.user_id,
        created_at=category.created_at
    )

@router.delete("/{category_id}", status_code=204, responses={404: {"model": ErrorResponse}})
def delete_category(
    category_id: int,
    current_user: UserDomain = Depends(get_current_user),
    controller: CategoryController = Depends(get_category_controller)
):
    try:
        controller.delete_category(category_id, current_user.id)
    except NotFoundError as e:
        raise HTTPException(status_code=404, detail=str(e))
    return
