using System.Text;
using System.Text.Json;
using Clients.Application;
using Clients.Domain;
using RabbitMQ.Client;
using Shared.Contracts;

namespace Clients.Infrastructure;

/// <summary>Publica eventos de cliente en RabbitMQ.</summary>
public sealed class RabbitMqClientEventPublisher(string host, string username, string password) : IClientEventPublisher
{
    public Task PublishAsync(Client client, CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory { HostName = host, UserName = username, Password = password };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare("devsu.events", ExchangeType.Topic, durable: true);
        var message = new ClientChangedEvent(Guid.NewGuid(), client.Id, client.FullName, client.IsActive, DateTimeOffset.UtcNow);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        channel.BasicPublish("devsu.events", "client.changed", null, body);
        return Task.CompletedTask;
    }
}