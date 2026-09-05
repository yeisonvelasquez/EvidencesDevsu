using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contracts;

namespace Accounts.Infrastructure;

/// <summary>Consume eventos de cliente y actualiza una proyección local idempotente.</summary>
public sealed class ClientChangedConsumer(IServiceScopeFactory scopeFactory, string host, string username, string password) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = host, UserName = username, Password = password };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        channel.ExchangeDeclare("devsu.events", ExchangeType.Topic, durable: true);
        channel.QueueDeclare("accounts.client-changed", durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind("accounts.client-changed", "devsu.events", "client.changed");
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (_, eventArgs) =>
        {
            var message = JsonSerializer.Deserialize<ClientChangedEvent>(Encoding.UTF8.GetString(eventArgs.Body.ToArray()));
            if (message is null) return;
            await using var scope = scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();
            var projection = await db.ClientProjections.SingleOrDefaultAsync(x => x.ClientId == message.ClientId, stoppingToken);
            if (projection is null) db.ClientProjections.Add(new ClientProjection { ClientId = message.ClientId, FullName = message.FullName, IsActive = message.IsActive, UpdatedAt = message.OccurredAt });
            else if (message.OccurredAt > projection.UpdatedAt) { projection.FullName = message.FullName; projection.IsActive = message.IsActive; projection.UpdatedAt = message.OccurredAt; }
            await db.SaveChangesAsync(stoppingToken);
            channel.BasicAck(eventArgs.DeliveryTag, false);
        };
        channel.BasicConsume("accounts.client-changed", autoAck: false, consumer);
        return Task.CompletedTask;
    }
}