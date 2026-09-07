using Accounts.Domain;

namespace Accounts.Application.Ports;

public interface IAccountCommandRepository
{
    Task<bool> IsClientActiveAsync(Guid clientId, CancellationToken cancellationToken);
    Task<Account?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByNumberAsync(string accountNumber, Guid? excludingId, CancellationToken cancellationToken);
    Task AddAsync(Account account, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}