from sqlalchemy.orm import Session
from sqlalchemy import func
from typing import List, Optional
from datetime import datetime
from clients.orm_models import Transaction
from models.domain_models import TransactionDomain
from controllers.transaction_repository_interface import ITransactionRepository

class TransactionRepository(ITransactionRepository):
    def __init__(self, session: Session):
        self._session = session

    def create(self, transaction: TransactionDomain) -> TransactionDomain:
        db_transaction = Transaction(
            amount=transaction.amount,
            description=transaction.description,
            date=transaction.date,
            from_account_id=transaction.from_account_id,
            to_account_id=transaction.to_account_id,
            category_id=transaction.category_id,
            user_id=transaction.user_id
        )
        self._session.add(db_transaction)
        self._session.commit()
        self._session.refresh(db_transaction)
        return self._to_domain(db_transaction)

    def get_by_id(self, transaction_id: int, user_id: int) -> Optional[TransactionDomain]:
        db_transaction = self._session.query(Transaction).filter(
            Transaction.id == transaction_id, Transaction.user_id == user_id
        ).first()
        return self._to_domain(db_transaction) if db_transaction else None

    def get_by_user(self, user_id: int, start_date: Optional[datetime] = None, end_date: Optional[datetime] = None) -> List[TransactionDomain]:
        query = self._session.query(Transaction).filter(Transaction.user_id == user_id)
        if start_date:
            query = query.filter(Transaction.date >= start_date)
        if end_date:
            query = query.filter(Transaction.date <= end_date)
        db_transactions = query.all()
        return [self._to_domain(t) for t in db_transactions]

    def delete(self, transaction_id: int, user_id: int) -> None:
        self._session.query(Transaction).filter(
            Transaction.id == transaction_id, Transaction.user_id == user_id
        ).delete()
        self._session.commit()

    def get_sum_by_category_and_month(self, category_id: int, user_id: int, month: int, year: int) -> float:
        result = self._session.query(func.sum(Transaction.amount)).filter(
            Transaction.category_id == category_id,
            Transaction.user_id == user_id,
            func.strftime('%m', Transaction.date) == f"{month:02d}",
            func.strftime('%Y', Transaction.date) == str(year),
            Transaction.to_account_id == None
        ).scalar()
        return result or 0.0

    def _to_domain(self, db_transaction: Transaction) -> TransactionDomain:
        return TransactionDomain(
            id=db_transaction.id,
            amount=db_transaction.amount,
            description=db_transaction.description,
            date=db_transaction.date,
            from_account_id=db_transaction.from_account_id,
            to_account_id=db_transaction.to_account_id,
            category_id=db_transaction.category_id,
            user_id=db_transaction.user_id
        )
