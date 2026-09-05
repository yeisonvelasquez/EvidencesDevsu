using Clients.Api;
using Clients.Application;
using Clients.Domain;
using Clients.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Clients.Api.xml")));
builder.Services.AddDbContext<ClientsDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ClientsDatabase")));
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IClientEventPublisher>(_ => new RabbitMqClientEventPublisher(builder.Configuration["RabbitMq:Host"] ?? "rabbitmq", builder.Configuration["RabbitMq:Username"] ?? "guest", builder.Configuration["RabbitMq:Password"] ?? "guest"));

var app = builder.Build();
app.UseMiddleware<ApiExceptionMiddleware>();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
await SeedAsync(app);
app.Run();

static async Task SeedAsync(WebApplication app)
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<ClientsDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (await db.Clients.AnyAsync()) return;
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    db.Clients.AddRange(
        new Client("Jose", "Lema", "M", 30, "0102030405", "Otalvaro sn y principal", "098254785", "jose.lema", hasher.Hash("1234"), Guid.Parse("10000000-0000-0000-0000-000000000001")),
        new Client("Marianela", "Montalvo", "F", 28, "0102030406", "Amazonas y NNUU", "097548965", "marianela.montalvo", hasher.Hash("5678"), Guid.Parse("10000000-0000-0000-0000-000000000002")),
        new Client("Juan", "Osorio", "M", 35, "0102030407", "13 junio y Equinoccial", "098874587", "juan.osorio", hasher.Hash("1245"), Guid.Parse("10000000-0000-0000-0000-000000000003")));
    await db.SaveChangesAsync();
}

public partial class Program;
