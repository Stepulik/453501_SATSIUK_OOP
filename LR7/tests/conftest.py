import pytest
from fastapi.testclient import TestClient
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker
from main import app
from clients.database import Base, get_db
from clients.orm_models import User, Account, Category, Transaction, Budget
import os

# Используем файловую БД для тестов (не in-memory)
TEST_DATABASE_URL = "sqlite:///./test.db"
engine = create_engine(TEST_DATABASE_URL, connect_args={"check_same_thread": False})
TestingSessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

def override_get_db():
    db = TestingSessionLocal()
    try:
        yield db
    finally:
        db.close()

app.dependency_overrides[get_db] = override_get_db

@pytest.fixture(autouse=True, scope="function")
def setup_database():
    # Перед каждым тестом создаём таблицы заново
    Base.metadata.drop_all(bind=engine)   # чистим от предыдущего теста
    Base.metadata.create_all(bind=engine)
    yield
    # После теста не удаляем, чтобы можно было посмотреть, но можно и удалить
    # Base.metadata.drop_all(bind=engine)  # раскомментировать, если нужно

@pytest.fixture
def client():
    return TestClient(app)

@pytest.fixture
def auth_token(client):
    # Регистрируем пользователя
    client.post("/api/v1/auth/register", json={
        "email": "test@example.com",
        "username": "testuser",
        "password": "testpass"
    })
    # Логинимся
    response = client.post("/api/v1/auth/login", json={
        "username": "testuser",
        "password": "testpass"
    })
    return response.json()["access_token"]

@pytest.fixture
def auth_headers(auth_token):
    return {"Authorization": f"Bearer {auth_token}"}
