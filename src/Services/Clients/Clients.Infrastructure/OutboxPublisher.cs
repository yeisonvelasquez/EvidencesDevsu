using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Clients.Infrastructure;

/// <summary>Publica eventos pendientes y conserva reintentos ante fallos del broker.</summary>
public sealed class OutboxPublisher(IServiceScopeFactory scopeFactory, string host, string username, string password) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await PublishPendingAsync(stoppingToken); }
            catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
            {
                Console.Error.WriteLine($"Outbox publisher error: {exception.Message}");
            }
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }

    private async Task PublishPendingAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ClientsDbContext>();
        var messages = await db.OutboxMessages.Where(x => x.ProcessedAt == null).OrderBy(x => x.OccurredAt).Take(50).ToListAsync(cancellationToken);
        if (messages.Count == 0) return;

        var factory = new ConnectionFactory { HostName = host, UserName = username, Password = password };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare("devsu.events", ExchangeType.Topic, durable: true);
        foreach (var message in messages)
        {
            try
            {
                channel.BasicPublish("devsu.events", message.EventType, null, Encoding.UTF8.GetBytes(message.Payload));
                message.ProcessedAt = DateTimeOffset.UtcNow;
            }
            catch (Exception exception)
            {
                message.Attempts++;
                message.LastError = exception.Message;
            }
        }
        await db.SaveChangesAsync(cancellationToken);
    }
}