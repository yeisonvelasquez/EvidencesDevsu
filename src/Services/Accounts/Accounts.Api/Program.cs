using Accounts.Api;
using Accounts.Application;
using Accounts.Application.Ports;
using Accounts.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Accounts.Api.xml")));
builder.Services.AddDbContext<AccountsDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("AccountsDatabase")));
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddHostedService(provider => new ClientChangedConsumer(provider.GetRequiredService<IServiceScopeFactory>(), builder.Configuration["RabbitMq:Host"] ?? "rabbitmq", builder.Configuration["RabbitMq:Username"] ?? "guest", builder.Configuration["RabbitMq:Password"] ?? "guest"));

var app = builder.Build();
app.UseMiddleware<ApiExceptionMiddleware>();
if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
await SeedAsync(app);
app.Run();

static async Task SeedAsync(WebApplication app)
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();
    await db.Database.EnsureCreatedAsync();
    await db.Database.ExecuteSqlRawAsync("ALTER TABLE client_projections ADD COLUMN IF NOT EXISTS identification varchar(40)");
    await db.Database.ExecuteSqlRawAsync("CREATE UNIQUE INDEX IF NOT EXISTS ix_client_projections_identification ON client_projections(identification) WHERE identification IS NOT NULL");
    var clients = new[]
    {
        Guid.Parse("10000000-0000-0000-0000-000000000001"),
        Guid.Parse("10000000-0000-0000-0000-000000000002"),
        Guid.Parse("10000000-0000-0000-0000-000000000003")
    };
    var clientSeeds = new[]
    {
        (ClientId: clients[0], FullName: "Jose Lema", Identification: "0102030405"),
        (ClientId: clients[1], FullName: "Marianela Montalvo", Identification: "0102030406"),
        (ClientId: clients[2], FullName: "Juan Osorio", Identification: "0102030407")
    };

    foreach (var seed in clientSeeds)
    {
        var projection = await db.ClientProjections.SingleOrDefaultAsync(x => x.ClientId == seed.ClientId);
        if (projection is null)
        {
            db.ClientProjections.Add(new ClientProjection { ClientId = seed.ClientId, FullName = seed.FullName, Identification = seed.Identification, IsActive = true, UpdatedAt = DateTimeOffset.UtcNow });
        }
        else if (projection.Identification is null)
        {
            projection.Identification = seed.Identification;
        }
    }
    await db.SaveChangesAsync();
    var accountSeeds = new[]
    {
        (AccountNumber: "478758", AccountType: "Savings", InitialBalance: 2000m, ClientId: clients[0]),
        (AccountNumber: "225487", AccountType: "Checking", InitialBalance: 100m, ClientId: clients[1]),
        (AccountNumber: "495878", AccountType: "Savings", InitialBalance: 0m, ClientId: clients[2]),
        (AccountNumber: "496825", AccountType: "Savings", InitialBalance: 540m, ClientId: clients[1]),
        (AccountNumber: "585545", AccountType: "Checking", InitialBalance: 1000m, ClientId: clients[0])
    };

    foreach (var seed in accountSeeds)
    {
        if (!await db.Accounts.AnyAsync(x => x.AccountNumber == seed.AccountNumber))
        {
            db.Accounts.Add(new Accounts.Domain.Account(seed.AccountNumber, seed.AccountType, seed.InitialBalance, seed.ClientId));
        }
    }
    await db.SaveChangesAsync();

    await db.Database.ExecuteSqlInterpolatedAsync($"""
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
        """);
}

public partial class Program;
