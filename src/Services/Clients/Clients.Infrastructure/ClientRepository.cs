using Clients.Application;
using Clients.Domain;
using Microsoft.EntityFrameworkCore;

namespace Clients.Infrastructure;

/// <summary>Repositorio EF Core para clientes.</summary>
public sealed class ClientRepository(ClientsDbContext dbContext) : IClientRepository
{
    public async Task<IReadOnlyList<Client>> ListAsync(CancellationToken cancellationToken) => await dbContext.Clients.AsNoTracking().OrderBy(x => x.LastName).ToListAsync(cancellationToken);
    public Task<Client?> GetAsync(Guid clientId, CancellationToken cancellationToken) => dbContext.Clients.SingleOrDefaultAsync(x => x.ClientId == clientId, cancellationToken);
    public Task<bool> ExistsByIdentificationAsync(string identification, Guid? excludingClientId, CancellationToken cancellationToken) => dbContext.Clients.AnyAsync(x => x.Identification == identification && (excludingClientId == null || x.ClientId != excludingClientId), cancellationToken);
    public Task AddAsync(Client client, CancellationToken cancellationToken) => dbContext.Clients.AddAsync(client, cancellationToken).AsTask();
    public void Remove(Client client) => dbContext.Clients.Remove(client);
    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}