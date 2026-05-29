from fastapi import APIRouter, Depends, HTTPException, Query
from sqlalchemy.orm import Session
from datetime import datetime
from typing import Optional, List, Dict
from clients.database import get_db
from clients.repositories.transaction_repo import TransactionRepository
from clients.repositories.category_repo import CategoryRepository
from api.dependencies import get_current_user
from models.domain_models import UserDomain
from shared.errors import DomainError

router = APIRouter()

@router.get("/expenses")
def get_expense_summary(
    start_date: datetime = Query(...),
    end_date: datetime = Query(...),
    current_user: UserDomain = Depends(get_current_user),
    db: Session = Depends(get_db)
):
    """Возвращает сумму расходов по категориям за период"""
    transaction_repo = TransactionRepository(db)
    category_repo = CategoryRepository(db)
    
    # Получаем все транзакции пользователя за период
    transactions = transaction_repo.get_by_user(current_user.id, start_date, end_date)
    # Фильтруем расходы (to_account_id is None и категория типа expense)
    expense_categories = {c.id: c.name for c in category_repo.get_by_user(current_user.id) if c.type == "expense"}
    summary = {}
    for t in transactions:
        if t.to_account_id is None and t.category_id and t.category_id in expense_categories:
            cat_name = expense_categories[t.category_id]
            summary[cat_name] = summary.get(cat_name, 0) + t.amount
    # Преобразуем в список для JSON
    result = [{"category": name, "total": amount} for name, amount in summary.items()]
    return {"start_date": start_date, "end_date": end_date, "summary": result}
