using Accounts.Api;
using Accounts.Application;
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
    if (await db.Accounts.AnyAsync()) return;
    var clients = new[]
    {
        Guid.Parse("10000000-0000-0000-0000-000000000001"),
        Guid.Parse("10000000-0000-0000-0000-000000000002"),
        Guid.Parse("10000000-0000-0000-0000-000000000003")
    };
    db.Accounts.AddRange(new Accounts.Domain.Account("478758", "Savings", 2000, clients[0]), new Accounts.Domain.Account("225487", "Checking", 100, clients[1]), new Accounts.Domain.Account("495878", "Savings", 0, clients[2]), new Accounts.Domain.Account("496825", "Savings", 540, clients[1]), new Accounts.Domain.Account("585545", "Checking", 1000, clients[0]));
    await db.SaveChangesAsync();
}

public partial class Program;
