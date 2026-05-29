from dataclasses import dataclass
from datetime import datetime
from typing import Optional

@dataclass
class UserDomain:
    id: Optional[int]
    email: str
    username: str
    hashed_password: str
    created_at: Optional[datetime] = None
    updated_at: Optional[datetime] = None

@dataclass
class AccountDomain:
    id: Optional[int]
    name: str
    balance: float
    user_id: int
    created_at: Optional[datetime] = None
    updated_at: Optional[datetime] = None

@dataclass
class CategoryDomain:
    id: Optional[int]
    name: str
    type: str   # "income" или "expense"
    user_id: int
    created_at: Optional[datetime] = None

@dataclass
class TransactionDomain:
    id: Optional[int]
    amount: float
    description: Optional[str]
    date: datetime
    from_account_id: int
    to_account_id: Optional[int]
    category_id: Optional[int]
    user_id: int

@dataclass
class BudgetDomain:
    id: Optional[int]
    category_id: int
    user_id: int
    month: int
    year: int
    limit_amount: float
    created_at: Optional[datetime] = None
    updated_at: Optional[datetime] = None
