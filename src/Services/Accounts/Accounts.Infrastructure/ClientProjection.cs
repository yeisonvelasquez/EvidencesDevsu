using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure;

/// <summary>Proyección local eventual de clientes recibida por eventos.</summary>
public sealed class ClientProjection
{
    public Guid ClientId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public partial class AccountsDbContext
{
    public DbSet<ClientProjection> ClientProjections => Set<ClientProjection>();
}