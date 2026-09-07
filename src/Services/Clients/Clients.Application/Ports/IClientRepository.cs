using Clients.Domain;

namespace Clients.Application.Ports;

/// <summary>Puerto de persistencia de clientes.</summary>
public interface IClientRepository
{
    Task<IReadOnlyList<Client>> ListAsync(CancellationToken cancellationToken);
    Task<Client?> GetAsync(Guid clientId, CancellationToken cancellationToken);
    Task<bool> ExistsByIdentificationAsync(string identification, Guid? excludingClientId, CancellationToken cancellationToken);
    Task AddAsync(Client client, CancellationToken cancellationToken);
    void Remove(Client client);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}