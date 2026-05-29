from sqlalchemy.orm import Session
from typing import List, Optional
from clients.orm_models import Budget
from models.domain_models import BudgetDomain
from controllers.budget_repository_interface import IBudgetRepository

class BudgetRepository(IBudgetRepository):
    def __init__(self, session: Session):
        self._session = session

    def create(self, budget: BudgetDomain) -> BudgetDomain:
        db_budget = Budget(
            category_id=budget.category_id,
            user_id=budget.user_id,
            month=budget.month,
            year=budget.year,
            limit_amount=budget.limit_amount
        )
        self._session.add(db_budget)
        self._session.commit()
        self._session.refresh(db_budget)
        return self._to_domain(db_budget)

    def get_by_id(self, budget_id: int, user_id: int) -> Optional[BudgetDomain]:
        db_budget = self._session.query(Budget).filter(Budget.id == budget_id, Budget.user_id == user_id).first()
        return self._to_domain(db_budget) if db_budget else None

    def get_by_user(self, user_id: int) -> List[BudgetDomain]:
        db_budgets = self._session.query(Budget).filter(Budget.user_id == user_id).all()
        return [self._to_domain(b) for b in db_budgets]

    def get_by_category_and_month(self, category_id: int, user_id: int, month: int, year: int) -> Optional[BudgetDomain]:
        db_budget = self._session.query(Budget).filter(
            Budget.category_id == category_id,
            Budget.user_id == user_id,
            Budget.month == month,
            Budget.year == year
        ).first()
        return self._to_domain(db_budget) if db_budget else None

    def delete(self, budget_id: int, user_id: int) -> None:
        self._session.query(Budget).filter(Budget.id == budget_id, Budget.user_id == user_id).delete()
        self._session.commit()

    def _to_domain(self, db_budget: Budget) -> BudgetDomain:
        return BudgetDomain(
            id=db_budget.id,
            category_id=db_budget.category_id,
            user_id=db_budget.user_id,
            month=db_budget.month,
            year=db_budget.year,
            limit_amount=db_budget.limit_amount,
            created_at=db_budget.created_at,
            updated_at=db_budget.updated_at
        )
