using Clients.Domain;
using Microsoft.EntityFrameworkCore;

namespace Clients.Infrastructure;

/// <summary>Contexto PostgreSQL del microservicio de clientes.</summary>
public sealed class ClientsDbContext(DbContextOptions<ClientsDbContext> options) : DbContext(options)
{
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("clients");
            entity.HasKey(x => x.ClientId);
            entity.Property(x => x.ClientId).HasColumnName("client_id");
            entity.Property(x => x.Identification).HasColumnName("identification").HasMaxLength(40).IsRequired();
            entity.HasIndex(x => x.Identification).IsUnique();
            entity.Property(x => x.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
            entity.Property(x => x.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Gender).HasColumnName("gender").HasMaxLength(30).IsRequired();
            entity.Property(x => x.Age).HasColumnName("age").IsRequired();
            entity.Property(x => x.Address).HasColumnName("address").HasMaxLength(250).IsRequired();
            entity.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(30).IsRequired();
            entity.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("outbox_messages");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.EventType).HasColumnName("event_type").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Payload).HasColumnName("payload").HasColumnType("jsonb").IsRequired();
            entity.Property(x => x.OccurredAt).HasColumnName("occurred_at").IsRequired();
            entity.Property(x => x.ProcessedAt).HasColumnName("processed_at");
            entity.Property(x => x.LastError).HasColumnName("last_error");
            entity.HasIndex(x => x.ProcessedAt);
        });
    }
}