# Трекер финансов

Простое API для учёта денег: счета, доходы/расходы, переводы, бюджеты, аналитика.  
Сделано на FastAPI, архитектура — чистая (по Роберту Мартину).

## Что умеет

- Регистрация и вход по JWT
- Счета (добавить, посмотреть, удалить)
- Категории (доход или расход)
- Транзакции: доход, расход, перевод между счетами (баланс сам пересчитывается)
- Бюджеты: лимит на категорию в месяц, проверка при тратах
- Отчёт по расходам за период (по категориям)
- Swagger по адресу /docs

## Как запустить

```bash
cd ~/Projects/Sem_4/OOP/453501_SATSIUK_OOP/LR7
source venv/bin/activate
alembic upgrade head
uvicorn main:app --reload
Сервер будет на http://localhost:8000

Переменные окружения (.env)
text
DATABASE_URL=sqlite:///./finance_tracker.db
SECRET_KEY=любой_ключ
ALGORITHM=HS256
ACCESS_TOKEN_EXPIRE_MINUTES=30
Структура проекта
api/ — ручки (хендлеры)

controllers/ — бизнес-логика (use cases)

clients/ — работа с БД (репозитории)

models/ — схемы данных (Pydantic, dataclass)

shared/ — общие утилиты (конфиг, jwt, ошибки)

tests/ — тесты

Зависимости идут снаружи внутрь: хендлеры -> контроллеры <- репозитории. Так проще тестировать и менять БД.

Паттерны, которые использовал
Репозиторий — чтобы не привязываться к SQLite, можно потом переделать на PostgreSQL.

DI (внедрение зависимостей) — через Depends у FastAPI, в тестах легко подменить репозиторий на мок.

DTO — отдельные классы для запросов и ответов, не таскаю доменные объекты наружу.

Use Case — каждый метод контроллера делает одно конкретное действие.

Запуск тестов
bash
pytest -v --cov=. --cov-report=term
Покрытие кода ~88%, все тесты проходят.

Примеры запросов
Регистрация
bash
curl -X POST /api/v1/auth/register -H "Content-Type: application/json" -d '{"email":"a@b.ru","username":"alex","password":"123"}'
Логин
bash
curl -X POST /api/v1/auth/login -H "Content-Type: application/json" -d '{"username":"alex","password":"123"}'
# получите токен
Создать счёт
bash
curl -X POST /api/v1/accounts/ -H "Authorization: Bearer <токен>" -H "Content-Type: application/json" -d '{"name":"Наличные","initial_balance":1000}'
Добавить расход
bash
curl -X POST /api/v1/transactions/ -H "Authorization: Bearer <токен>" -d '{"amount":500,"from_account_id":1,"category_id":2}'
Аналитика за май
bash
curl -X GET "/api/v1/analytics/expenses?start_date=2025-05-01T00:00:00&end_date=2025-05-31T23:59:59" -H "Authorization: Bearer <токен>"
Всё, можно пользоваться.
