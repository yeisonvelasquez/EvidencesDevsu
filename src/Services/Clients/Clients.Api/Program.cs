using Clients.Api;
using Clients.Application;
using Clients.Application.Ports;
using Clients.Application.Validators;
using Clients.Infrastructure;
using Clients.Infrastructure.Seeding;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateClientRequestValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Clients.Api.xml")));
builder.Services.AddDbContext<ClientsDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ClientsDatabase")));
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IClientEventStore, ClientEventStore>();
builder.Services.AddScoped<ClientsDatabaseSeeder>();
builder.Services.AddHostedService(provider => new OutboxPublisher(provider.GetRequiredService<IServiceScopeFactory>(), builder.Configuration["RabbitMq:Host"] ?? "rabbitmq", builder.Configuration["RabbitMq:Username"] ?? "guest", builder.Configuration["RabbitMq:Password"] ?? "guest"));

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
    await scope.ServiceProvider.GetRequiredService<ClientsDatabaseSeeder>().SeedAsync();
}
app.Run();

public partial class Program;
