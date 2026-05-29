import asyncio
from logging.config import fileConfig
from sqlalchemy import pool, engine_from_config
from sqlalchemy.ext.asyncio import AsyncEngine
from alembic import context

import sys
from pathlib import Path
sys.path.append(str(Path(__file__).parent.parent))

from clients.database import Base
from clients.orm_models import User

config = context.config
fileConfig(config.config_file_name)
target_metadata = Base.metadata

def run_migrations_offline():
    url = config.get_main_option("sqlalchemy.url")
    context.configure(url=url, target_metadata=target_metadata, literal_binds=True)
    with context.begin_transaction():
        context.run_migrations()

def do_run_migrations(connection):
    context.configure(connection=connection, target_metadata=target_metadata)

async def run_migrations_online():
    config_section = config.get_section(config.config_ini_section)
    config_section["sqlalchemy.url"] = config.get_main_option("sqlalchemy.url")
    connectable = engine_from_config(
        config_section,
        prefix="sqlalchemy.",
        poolclass=pool.NullPool,
        future=True,
    )
    with connectable.connect() as connection:
        context.configure(connection=connection, target_metadata=target_metadata)
        with context.begin_transaction():
            context.run_migrations()

if context.is_offline_mode():
    run_migrations_offline()
else:
    asyncio.run(run_migrations_online())
