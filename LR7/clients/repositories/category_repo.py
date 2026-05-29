from sqlalchemy.orm import Session
from typing import List, Optional
from clients.orm_models import Category
from models.domain_models import CategoryDomain
from controllers.category_repository_interface import ICategoryRepository

class CategoryRepository(ICategoryRepository):
    def __init__(self, session: Session):
        self._session = session

    def create(self, category: CategoryDomain) -> CategoryDomain:
        db_category = Category(
            name=category.name,
            type=category.type,
            user_id=category.user_id
        )
        self._session.add(db_category)
        self._session.commit()
        self._session.refresh(db_category)
        return self._to_domain(db_category)

    def get_by_id(self, category_id: int, user_id: int) -> Optional[CategoryDomain]:
        db_category = self._session.query(Category).filter(Category.id == category_id, Category.user_id == user_id).first()
        return self._to_domain(db_category) if db_category else None

    def get_by_user(self, user_id: int) -> List[CategoryDomain]:
        db_categories = self._session.query(Category).filter(Category.user_id == user_id).all()
        return [self._to_domain(c) for c in db_categories]

    def get_by_name_and_type(self, user_id: int, name: str, type: str) -> Optional[CategoryDomain]:
        db_category = self._session.query(Category).filter(Category.user_id == user_id, Category.name == name, Category.type == type).first()
        return self._to_domain(db_category) if db_category else None

    def delete(self, category_id: int, user_id: int) -> None:
        self._session.query(Category).filter(Category.id == category_id, Category.user_id == user_id).delete()
        self._session.commit()

    def _to_domain(self, db_category: Category) -> CategoryDomain:
        return CategoryDomain(
            id=db_category.id,
            name=db_category.name,
            type=db_category.type,
            user_id=db_category.user_id,
            created_at=db_category.created_at
        )
