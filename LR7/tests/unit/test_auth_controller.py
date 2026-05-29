import pytest
from unittest.mock import AsyncMock, MagicMock
from controllers.auth_controller import AuthController
from models.request_models import UserRegisterRequest, UserLoginRequest
from models.domain_models import UserDomain
from shared.errors import AlreadyExistsError, UnauthorizedError

@pytest.fixture
def mock_user_repo():
    return MagicMock()

@pytest.fixture
def auth_controller(mock_user_repo):
    return AuthController(mock_user_repo)

def test_register_success(auth_controller, mock_user_repo):
    mock_user_repo.get_by_username.return_value = None
    mock_user_repo.get_by_email.return_value = None
    mock_user_repo.create.return_value = UserDomain(id=1, email="new@example.com", username="newuser", hashed_password="hashed")
    req = UserRegisterRequest(email="new@example.com", username="newuser", password="pass123")
    user = auth_controller.register(req)
    assert user.id == 1
    mock_user_repo.create.assert_called_once()

def test_register_username_exists(auth_controller, mock_user_repo):
    mock_user_repo.get_by_username.return_value = UserDomain(id=1, email="old@example.com", username="newuser", hashed_password="...")
    req = UserRegisterRequest(email="new@example.com", username="newuser", password="pass123")
    with pytest.raises(AlreadyExistsError):
        auth_controller.register(req)

def test_login_success(auth_controller, mock_user_repo):
    from shared.security import get_password_hash
    hashed = get_password_hash("correct")
    mock_user_repo.get_by_username.return_value = UserDomain(id=1, email="test@example.com", username="testuser", hashed_password=hashed)
    req = UserLoginRequest(username="testuser", password="correct")
    token = auth_controller.login(req)
    assert token is not None and isinstance(token, str)

def test_login_invalid_password(auth_controller, mock_user_repo):
    from shared.security import get_password_hash
    hashed = get_password_hash("correct")
    mock_user_repo.get_by_username.return_value = UserDomain(id=1, email="test@example.com", username="testuser", hashed_password=hashed)
    req = UserLoginRequest(username="testuser", password="wrong")
    with pytest.raises(UnauthorizedError):
        auth_controller.login(req)

def test_login_user_not_found(auth_controller, mock_user_repo):
    mock_user_repo.get_by_username.return_value = None
    req = UserLoginRequest(username="unknown", password="anything")
    with pytest.raises(UnauthorizedError):
        auth_controller.login(req)
