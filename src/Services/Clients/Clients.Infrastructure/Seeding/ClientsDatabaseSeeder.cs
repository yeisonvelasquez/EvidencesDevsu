using Clients.Application.Ports;
using Clients.Domain;
using Microsoft.EntityFrameworkCore;

namespace Clients.Infrastructure.Seeding;

/// <summary>Inicializa el esquema y los datos de ejemplo del servicio de clientes.</summary>
public sealed class ClientsDatabaseSeeder(ClientsDbContext db, IPasswordHasher passwordHasher)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await db.Database.EnsureCreatedAsync(cancellationToken);
        if (await db.Clients.AnyAsync(cancellationToken)) return;

        db.Clients.AddRange(
            new Client(Guid.Parse("10000000-0000-0000-0000-000000000001"), "Jose", "Lema", "M", 30, "0102030405", "Otalvaro sn y principal", "098254785", passwordHasher.Hash("1234")),
            new Client(Guid.Parse("10000000-0000-0000-0000-000000000002"), "Marianela", "Montalvo", "F", 28, "0102030406", "Amazonas y NNUU", "097548965", passwordHasher.Hash("5678")),
            new Client(Guid.Parse("10000000-0000-0000-0000-000000000003"), "Juan", "Osorio", "M", 35, "0102030407", "13 junio y Equinoccial", "098874587", passwordHasher.Hash("1245")));

        await db.SaveChangesAsync(cancellationToken);
    }
}