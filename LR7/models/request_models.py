from pydantic import BaseModel, EmailStr
from datetime import datetime
from typing import Optional

class UserRegisterRequest(BaseModel):
    email: EmailStr
    username: str
    password: str

class UserLoginRequest(BaseModel):
    username: str
    password: str

class AccountCreateRequest(BaseModel):
    name: str
    initial_balance: float = 0.0

class AccountUpdateRequest(BaseModel):
    name: Optional[str] = None

class CategoryCreateRequest(BaseModel):
    name: str
    type: str   # "income" или "expense"

class TransactionCreateRequest(BaseModel):
    amount: float
    description: Optional[str] = None
    from_account_id: int
    to_account_id: Optional[int] = None   # если None, то доход/расход (to_account = from_account для дохода? уточним)
    category_id: Optional[int] = None
    date: Optional[datetime] = None

class BudgetCreateRequest(BaseModel):
    category_id: int
    month: int
    year: int
    limit_amount: float

class BudgetCreateRequest(BaseModel):
    category_id: int
    month: int
    year: int
    limit_amount: float
