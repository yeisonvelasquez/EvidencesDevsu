CREATE EXTENSION IF NOT EXISTS "pgcrypto";

CREATE TABLE IF NOT EXISTS clients (
    client_id uuid PRIMARY KEY,
    first_name varchar(100) NOT NULL,
    last_name varchar(100) NOT NULL,
    gender varchar(30) NOT NULL,
    age integer NOT NULL CHECK (age BETWEEN 0 AND 130),
    identification varchar(40) NOT NULL UNIQUE,
    address varchar(250) NOT NULL,
    phone varchar(30) NOT NULL,
    password_hash varchar(255) NOT NULL,
    is_active boolean NOT NULL DEFAULT TRUE,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NULL
);

CREATE TABLE IF NOT EXISTS accounts (
    id uuid PRIMARY KEY,
    account_number varchar(30) NOT NULL UNIQUE,
    account_type varchar(30) NOT NULL,
    balance numeric(18,2) NOT NULL CHECK (balance >= 0),
    is_active boolean NOT NULL DEFAULT TRUE,
    client_id uuid NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NULL,
    version integer NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS ix_accounts_client_id ON accounts(client_id);

CREATE TABLE IF NOT EXISTS transactions (
    id uuid PRIMARY KEY,
    account_id uuid NOT NULL REFERENCES accounts(id),
    transaction_type varchar(20) NOT NULL CHECK (transaction_type IN ('Deposit', 'Withdrawal')),
    amount numeric(18,2) NOT NULL CHECK (amount > 0),
    previous_balance numeric(18,2) NOT NULL CHECK (previous_balance >= 0),
    resulting_balance numeric(18,2) NOT NULL CHECK (resulting_balance >= 0),
    occurred_at timestamptz NOT NULL,
    idempotency_key varchar(100) NULL UNIQUE
);
CREATE INDEX IF NOT EXISTS ix_transactions_account_date ON transactions(account_id, occurred_at);

CREATE TABLE IF NOT EXISTS client_projections (
    client_id uuid PRIMARY KEY,
    full_name varchar(220) NOT NULL,
    is_active boolean NOT NULL,
    updated_at timestamptz NOT NULL
);

CREATE TABLE IF NOT EXISTS outbox_messages (
    id uuid PRIMARY KEY,
    event_type varchar(100) NOT NULL,
    payload jsonb NOT NULL,
    occurred_at timestamptz NOT NULL,
    processed_at timestamptz NULL,
    attempts integer NOT NULL DEFAULT 0,
    last_error text NULL
);
CREATE INDEX IF NOT EXISTS ix_outbox_messages_pending ON outbox_messages(processed_at, occurred_at) WHERE processed_at IS NULL;