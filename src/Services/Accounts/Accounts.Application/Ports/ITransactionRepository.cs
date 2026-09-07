using Accounts.Domain;

namespace Accounts.Application.Ports;

public interface ITransactionRepository
{
    Task<Account?> GetAccountForUpdateAsync(string accountNumber, CancellationToken cancellationToken);
    Task<bool> IsClientActiveAsync(Guid clientId, CancellationToken cancellationToken);
    Task<Transaction?> GetByIdempotencyKeyAsync(string key, CancellationToken cancellationToken);
    Task AddAsync(Transaction transaction, string? idempotencyKey, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}