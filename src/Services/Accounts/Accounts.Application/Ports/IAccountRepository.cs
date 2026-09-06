namespace Accounts.Application.Ports;

using Accounts.Application.ReadModels;
using Accounts.Domain;

/// <summary>
/// Puerto de persistencia para cuentas y movimientos.
/// </summary>
public interface IAccountRepository
{
    Task<bool> IsClientActiveAsync(Guid clientId, CancellationToken cancellationToken);

    Task<IReadOnlyList<AccountReadModel>> ListAsync(Guid? clientId, CancellationToken cancellationToken);

    Task<Account?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<Account?> GetByNumberForUpdateAsync(string accountNumber, CancellationToken cancellationToken);

    Task<AccountReadModel?> GetByNumberAsync(string accountNumber, CancellationToken cancellationToken);

    Task<bool> ExistsByNumberAsync(string accountNumber, Guid? excludingId, CancellationToken cancellationToken);

    Task AddAsync(Account account, CancellationToken cancellationToken);

    Task<IReadOnlyList<Account>> GetStatementAsync(Guid clientId, DateTimeOffset start, DateTimeOffset end, CancellationToken cancellationToken);

    Task<Guid?> GetClientIdByIdentificationAsync(string identification, CancellationToken cancellationToken);

    Task<Transaction?> GetTransactionByIdempotencyKeyAsync(string key, CancellationToken cancellationToken);

    Task AddTransactionAsync(Transaction transaction, string? idempotencyKey, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
