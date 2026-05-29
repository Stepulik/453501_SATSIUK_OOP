import pytest
from unittest.mock import MagicMock
from controllers.account_controller import AccountController
from models.request_models import AccountCreateRequest
from models.domain_models import AccountDomain
from shared.errors import NotFoundError

@pytest.fixture
def mock_account_repo():
    return MagicMock()

@pytest.fixture
def account_controller(mock_account_repo):
    return AccountController(mock_account_repo)

def test_create_account(account_controller, mock_account_repo):
    req = AccountCreateRequest(name="Test", initial_balance=100.0)
    mock_account_repo.create.return_value = AccountDomain(id=1, name="Test", balance=100.0, user_id=10)
    account = account_controller.create_account(10, req)
    assert account.id == 1
    mock_account_repo.create.assert_called_once()

def test_get_user_accounts(account_controller, mock_account_repo):
    mock_account_repo.get_by_user.return_value = [AccountDomain(id=1, name="A", balance=0, user_id=10)]
    accounts = account_controller.get_user_accounts(10)
    assert len(accounts) == 1
    mock_account_repo.get_by_user.assert_called_with(10)

def test_get_account_not_found(account_controller, mock_account_repo):
    mock_account_repo.get_by_id.return_value = None
    with pytest.raises(NotFoundError):
        account_controller.get_account(999, 10)

def test_delete_account_calls_repo(account_controller, mock_account_repo):
    mock_account_repo.get_by_id.return_value = AccountDomain(id=1, name="A", balance=0, user_id=10)
    account_controller.delete_account(1, 10)
    mock_account_repo.delete.assert_called_with(1, 10)
