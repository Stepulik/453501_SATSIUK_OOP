from pydantic import BaseModel
from datetime import datetime
from typing import Optional

class TokenResponse(BaseModel):
    access_token: str
    token_type: str = "bearer"

class UserResponse(BaseModel):
    id: int
    email: str
    username: str

class ErrorResponse(BaseModel):
    detail: str

class AccountResponse(BaseModel):
    id: int
    name: str
    balance: float
    user_id: int
    created_at: datetime
    updated_at: Optional[datetime]

class CategoryResponse(BaseModel):
    id: int
    name: str
    type: str
    user_id: int
    created_at: datetime

class TransactionResponse(BaseModel):
    id: int
    amount: float
    description: Optional[str]
    date: datetime
    from_account_id: int
    to_account_id: Optional[int]
    category_id: Optional[int]
    user_id: int

class BudgetResponse(BaseModel):
    id: int
    category_id: int
    user_id: int
    month: int
    year: int
    limit_amount: float
    created_at: datetime
    updated_at: Optional[datetime]
