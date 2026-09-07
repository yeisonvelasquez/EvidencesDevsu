using Clients.Application.Contracts;

namespace Clients.Application.Ports;

/// <summary>Casos de uso del ciclo de vida del cliente.</summary>
public interface IClientService
{
    Task<IReadOnlyList<ClientResponse>> ListAsync(CancellationToken cancellationToken);
    Task<ClientResponse> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<ClientResponse> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken);
    Task<ClientResponse> UpdateAsync(Guid id, UpdateClientRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}