using Clients.Domain;

namespace Clients.Application.Ports;

/// <summary>Puerto para almacenar eventos de integración.</summary>
public interface IClientEventStore
{
    Task EnqueueAsync(Client client, CancellationToken cancellationToken);
}