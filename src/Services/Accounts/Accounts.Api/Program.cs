using Accounts.Api;
using Accounts.Application;
using Accounts.Application.Ports;
using Accounts.Application.Validators;
using Accounts.Infrastructure;
using Accounts.Infrastructure.Seeding;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAccountRequestValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Accounts.Api.xml")));
builder.Services.AddDbContext<AccountsDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("AccountsDatabase")));
builder.Services.AddScoped<IAccountReadRepository, AccountReadRepository>();
builder.Services.AddScoped<IAccountCommandRepository, AccountCommandRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IStatementRepository, StatementRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IStatementService, StatementService>();
builder.Services.AddScoped<AccountsDatabaseSeeder>();
builder.Services.AddHostedService(provider => new ClientChangedConsumer(provider.GetRequiredService<IServiceScopeFactory>(), builder.Configuration["RabbitMq:Host"] ?? "rabbitmq", builder.Configuration["RabbitMq:Username"] ?? "guest", builder.Configuration["RabbitMq:Password"] ?? "guest"));

var app = builder.Build();
app.UseMiddleware<ApiExceptionMiddleware>();
if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
await using (var scope = app.Services.CreateAsyncScope())
{
    await scope.ServiceProvider.GetRequiredService<AccountsDatabaseSeeder>().SeedAsync();
}
app.Run();

public partial class Program;
