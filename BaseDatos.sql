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
    identification varchar(40) NULL UNIQUE,
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

-- Movimientos iniciales solicitados. Requiere que las cuentas 225487 y 496825 ya existan.
DO $$
DECLARE
    checking_account_id uuid;
    savings_account_id uuid;
BEGIN
    SELECT id INTO checking_account_id FROM accounts WHERE account_number = '225487';
    IF checking_account_id IS NOT NULL AND NOT EXISTS (
        SELECT 1 FROM transactions WHERE idempotency_key = 'seed-225487-20220210'
    ) THEN
        INSERT INTO transactions (
            id, account_id, transaction_type, amount, previous_balance,
            resulting_balance, occurred_at, idempotency_key
        ) VALUES (
            '22548700-0000-0000-0000-000000000001', checking_account_id,
            'Deposit', 600.00, 100.00, 700.00,
            '2022-02-10T00:00:00Z', 'seed-225487-20220210'
        );
        UPDATE accounts SET balance = 700.00
        WHERE id = checking_account_id AND balance = 100.00;
    END IF;

    SELECT id INTO savings_account_id FROM accounts WHERE account_number = '496825';
    IF savings_account_id IS NOT NULL AND NOT EXISTS (
        SELECT 1 FROM transactions WHERE idempotency_key = 'seed-496825-20220208'
    ) THEN
        INSERT INTO transactions (
            id, account_id, transaction_type, amount, previous_balance,
            resulting_balance, occurred_at, idempotency_key
        ) VALUES (
            '49682500-0000-0000-0000-000000000001', savings_account_id,
            'Withdrawal', 540.00, 540.00, 0.00,
            '2022-02-08T00:00:00Z', 'seed-496825-20220208'
        );
        UPDATE accounts SET balance = 0.00
        WHERE id = savings_account_id AND balance = 540.00;
    END IF;
END $$;