using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure.Seeding;

/// <summary>Inicializa el esquema y los datos de ejemplo del servicio de cuentas.</summary>
public sealed class AccountsDatabaseSeeder(AccountsDbContext db)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await db.Database.EnsureCreatedAsync(cancellationToken);
        await EnsureProjectionSchemaAsync(cancellationToken);

        var clients = new[]
        {
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            Guid.Parse("10000000-0000-0000-0000-000000000002"),
            Guid.Parse("10000000-0000-0000-0000-000000000003")
        };

        await SeedClientProjectionsAsync(clients, cancellationToken);
        await SeedAccountsAsync(clients, cancellationToken);
        await SeedInitialTransactionsAsync(cancellationToken);
    }

    private async Task EnsureProjectionSchemaAsync(CancellationToken cancellationToken)
    {
        await db.Database.ExecuteSqlRawAsync("ALTER TABLE client_projections ADD COLUMN IF NOT EXISTS identification varchar(40)", cancellationToken);
        await db.Database.ExecuteSqlRawAsync("CREATE UNIQUE INDEX IF NOT EXISTS ix_client_projections_identification ON client_projections(identification) WHERE identification IS NOT NULL", cancellationToken);
    }

    private async Task SeedClientProjectionsAsync(Guid[] clients, CancellationToken cancellationToken)
    {
        var seeds = new[]
        {
            (ClientId: clients[0], FullName: "Jose Lema", Identification: "0102030405"),
            (ClientId: clients[1], FullName: "Marianela Montalvo", Identification: "0102030406"),
            (ClientId: clients[2], FullName: "Juan Osorio", Identification: "0102030407")
        };

        foreach (var seed in seeds)
        {
            var projection = await db.ClientProjections.SingleOrDefaultAsync(x => x.ClientId == seed.ClientId, cancellationToken);
            if (projection is null)
            {
                db.ClientProjections.Add(new ClientProjection { ClientId = seed.ClientId, FullName = seed.FullName, Identification = seed.Identification, IsActive = true, UpdatedAt = DateTimeOffset.UtcNow });
            }
            else if (projection.Identification is null)
            {
                projection.Identification = seed.Identification;
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedAccountsAsync(Guid[] clients, CancellationToken cancellationToken)
    {
        var seeds = new[]
        {
            (AccountNumber: "478758", AccountType: "Savings", InitialBalance: 2000m, ClientId: clients[0]),
            (AccountNumber: "225487", AccountType: "Checking", InitialBalance: 100m, ClientId: clients[1]),
            (AccountNumber: "495878", AccountType: "Savings", InitialBalance: 0m, ClientId: clients[2]),
            (AccountNumber: "496825", AccountType: "Savings", InitialBalance: 540m, ClientId: clients[1]),
            (AccountNumber: "585545", AccountType: "Checking", InitialBalance: 1000m, ClientId: clients[0])
        };

        foreach (var seed in seeds)
        {
            if (!await db.Accounts.AnyAsync(x => x.AccountNumber == seed.AccountNumber, cancellationToken))
            {
                db.Accounts.Add(new Accounts.Domain.Account(seed.AccountNumber, seed.AccountType, seed.InitialBalance, seed.ClientId));
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private Task<int> SeedInitialTransactionsAsync(CancellationToken cancellationToken)
    {
        return db.Database.ExecuteSqlRawAsync("""
            INSERT INTO transactions (id, "AccountId", transaction_type, amount, previous_balance, resulting_balance, occurred_at, idempotency_key)
            SELECT gen_random_uuid(), id, 'Deposit', 600.00, 100.00, 700.00, TIMESTAMPTZ '2022-02-10 00:00:00+00', 'seed-225487-20220210'
            FROM accounts
            WHERE account_number = '225487'
              AND NOT EXISTS (SELECT 1 FROM transactions WHERE idempotency_key = 'seed-225487-20220210');

            UPDATE accounts
            SET balance = 700.00
            WHERE account_number = '225487' AND balance = 100.00;

            INSERT INTO transactions (id, "AccountId", transaction_type, amount, previous_balance, resulting_balance, occurred_at, idempotency_key)
            SELECT gen_random_uuid(), id, 'Withdrawal', 540.00, 540.00, 0.00, TIMESTAMPTZ '2022-02-08 00:00:00+00', 'seed-496825-20220208'
            FROM accounts
            WHERE account_number = '496825'
              AND NOT EXISTS (SELECT 1 FROM transactions WHERE idempotency_key = 'seed-496825-20220208');

            UPDATE accounts
            SET balance = 0.00
            WHERE account_number = '496825' AND balance = 540.00;
            """, cancellationToken);
    }
}