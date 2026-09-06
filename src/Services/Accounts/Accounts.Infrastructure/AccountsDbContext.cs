using Accounts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure;

/// <summary>Contexto PostgreSQL del microservicio de cuentas.</summary>
public sealed partial class AccountsDbContext(DbContextOptions<AccountsDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("accounts");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.AccountNumber).HasColumnName("account_number").HasMaxLength(30).IsRequired();
            entity.HasIndex(x => x.AccountNumber).IsUnique();
            entity.Property(x => x.AccountType).HasColumnName("account_type").HasMaxLength(30).IsRequired();
            entity.Property(x => x.Balance).HasColumnName("balance").HasPrecision(18, 2).IsRequired();
            entity.Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
            entity.Property(x => x.ClientId).HasColumnName("client_id").IsRequired();
            entity.HasIndex(x => x.ClientId);
            entity.Property(x => x.Version).HasColumnName("version").IsConcurrencyToken();
            entity.HasMany(x => x.Transactions).WithOne().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("transactions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Type).HasColumnName("transaction_type").HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(x => x.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
            entity.Property(x => x.PreviousBalance).HasColumnName("previous_balance").HasPrecision(18, 2).IsRequired();
            entity.Property(x => x.ResultingBalance).HasColumnName("resulting_balance").HasPrecision(18, 2).IsRequired();
            entity.Property(x => x.OccurredAt).HasColumnName("occurred_at").IsRequired();
            entity.Property<string?>("IdempotencyKey").HasColumnName("idempotency_key").HasMaxLength(100);
            entity.HasIndex("IdempotencyKey").IsUnique().HasFilter("idempotency_key IS NOT NULL");
            entity.HasIndex(x => new { x.AccountId, x.OccurredAt });
        });
        modelBuilder.Entity<ClientProjection>(entity =>
        {
            entity.ToTable("client_projections");
            entity.HasKey(x => x.ClientId);
            entity.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(220).IsRequired();
            entity.Property(x => x.Identification).HasColumnName("identification").HasMaxLength(40);
            entity.HasIndex(x => x.Identification).IsUnique().HasFilter("identification IS NOT NULL");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        });
    }
}