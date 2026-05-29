from typing import List
from models.domain_models import CategoryDomain
from models.request_models import CategoryCreateRequest
from controllers.category_repository_interface import ICategoryRepository
from shared.errors import AlreadyExistsError, NotFoundError

class CategoryController:
    def __init__(self, category_repo: ICategoryRepository):
        self._category_repo = category_repo

    def create_category(self, user_id: int, req: CategoryCreateRequest) -> CategoryDomain:
        existing = self._category_repo.get_by_name_and_type(user_id, req.name, req.type)
        if existing:
            raise AlreadyExistsError(f"Category '{req.name}' with type '{req.type}' already exists")
        category = CategoryDomain(
            id=None,
            name=req.name,
            type=req.type,
            user_id=user_id
        )
        return self._category_repo.create(category)

    def get_user_categories(self, user_id: int) -> List[CategoryDomain]:
        return self._category_repo.get_by_user(user_id)

    def get_category(self, category_id: int, user_id: int) -> CategoryDomain:
        category = self._category_repo.get_by_id(category_id, user_id)
        if not category:
            raise NotFoundError(f"Category {category_id} not found")
        return category

    def delete_category(self, category_id: int, user_id: int) -> None:
        category = self.get_category(category_id, user_id)
        self._category_repo.delete(category_id, user_id)
