using System.Text.Json;
using Clients.Application.Ports;
using Clients.Domain;
using Shared.Contracts;

namespace Clients.Infrastructure;

/// <summary>Guarda eventos en el Outbox dentro de la misma transacción de EF Core.</summary>
public sealed class ClientEventStore(ClientsDbContext dbContext) : IClientEventStore
{
    public Task EnqueueAsync(Client client, CancellationToken cancellationToken)
    {
        var message = new ClientChangedEvent(Guid.NewGuid(), client.ClientId, client.FullName, client.Identification, client.IsActive, DateTimeOffset.UtcNow);
        dbContext.OutboxMessages.Add(new OutboxMessage
        {
            Id = message.MessageId,
            EventType = "client.changed",
            Payload = JsonSerializer.Serialize(message),
            OccurredAt = message.OccurredAt
        });
        return Task.CompletedTask;
    }
}