import pytest
from fastapi.testclient import TestClient
from main import app

client = TestClient(app)

def test_register_and_login():
    # Регистрация
    reg = client.post("/api/v1/auth/register", json={"email": "int@example.com", "username": "intuser", "password": "intpass"})
    assert reg.status_code == 200
    assert reg.json()["username"] == "intuser"
    # Логин
    login = client.post("/api/v1/auth/login", json={"username": "intuser", "password": "intpass"})
    assert login.status_code == 200
    token = login.json()["access_token"]
    # Создание счёта
    headers = {"Authorization": f"Bearer {token}"}
    acc = client.post("/api/v1/accounts/", headers=headers, json={"name": "Test Account", "initial_balance": 200.0})
    assert acc.status_code == 200
    assert acc.json()["name"] == "Test Account"
    # Получение счетов
    accs = client.get("/api/v1/accounts/", headers=headers)
    assert accs.status_code == 200
    assert len(accs.json()) == 1
    # Удаление счёта
    delete = client.delete(f"/api/v1/accounts/{acc.json()['id']}", headers=headers)
    assert delete.status_code == 204

def test_create_category_and_budget(auth_headers, client):
    # Создать категорию
    cat = client.post("/api/v1/categories/", headers=auth_headers, json={"name": "Food", "type": "expense"})
    assert cat.status_code == 200
    cat_id = cat.json()["id"]
    # Создать бюджет
    budget = client.post("/api/v1/budgets/", headers=auth_headers, json={"category_id": cat_id, "month": 5, "year": 2026, "limit_amount": 500})
    assert budget.status_code == 200
    # Получить бюджеты
    budgets = client.get("/api/v1/budgets/", headers=auth_headers)
    assert budgets.status_code == 200
    assert len(budgets.json()) == 1
    # Удалить бюджет
    del_b = client.delete(f"/api/v1/budgets/{budget.json()['id']}", headers=auth_headers)
    assert del_b.status_code == 204

def test_transaction_flow(auth_headers, client):
    # Создать счёт
    acc1 = client.post("/api/v1/accounts/", headers=auth_headers, json={"name": "Cash", "initial_balance": 1000})
    acc2 = client.post("/api/v1/accounts/", headers=auth_headers, json={"name": "Card", "initial_balance": 2000})
    acc1_id = acc1.json()["id"]
    acc2_id = acc2.json()["id"]
    # Создать категорию дохода и расхода
    inc_cat = client.post("/api/v1/categories/", headers=auth_headers, json={"name": "Salary", "type": "income"})
    exp_cat = client.post("/api/v1/categories/", headers=auth_headers, json={"name": "Groceries", "type": "expense"})
    inc_id = inc_cat.json()["id"]
    exp_id = exp_cat.json()["id"]
    # Доход
    inc_tx = client.post("/api/v1/transactions/", headers=auth_headers, json={"amount": 500, "from_account_id": acc1_id, "category_id": inc_id, "description": "salary"})
    assert inc_tx.status_code == 200
    # Расход
    exp_tx = client.post("/api/v1/transactions/", headers=auth_headers, json={"amount": 100, "from_account_id": acc2_id, "category_id": exp_id, "description": "food"})
    assert exp_tx.status_code == 200
    # Перевод
    transfer = client.post("/api/v1/transactions/", headers=auth_headers, json={"amount": 300, "from_account_id": acc2_id, "to_account_id": acc1_id, "description": "transfer"})
    assert transfer.status_code == 200
    # Проверить балансы
    final_acc1 = client.get(f"/api/v1/accounts/{acc1_id}", headers=auth_headers)
    final_acc2 = client.get(f"/api/v1/accounts/{acc2_id}", headers=auth_headers)
    assert final_acc1.json()["balance"] == 1000 + 500 + 300  # 1800
    assert final_acc2.json()["balance"] == 2000 - 100 - 300  # 1600
    # Проверить аналитику расходов
    analytics = client.get("/api/v1/analytics/expenses?start_date=2026-05-01T00:00:00&end_date=2026-05-31T23:59:59", headers=auth_headers)
    assert analytics.status_code == 200
    # Найдём сумму по категории Groceries
    summary = analytics.json()["summary"]
    groceries_total = next((item["total"] for item in summary if item["category"] == "Groceries"), 0)
    assert groceries_total == 100
