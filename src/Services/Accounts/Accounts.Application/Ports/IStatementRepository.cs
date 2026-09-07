using Accounts.Domain;

namespace Accounts.Application.Ports;

public interface IStatementRepository
{
    Task<Guid?> GetClientIdByIdentificationAsync(string identification, CancellationToken cancellationToken);
    Task<IReadOnlyList<Account>> GetAccountsAsync(Guid clientId, DateTimeOffset start, DateTimeOffset end, CancellationToken cancellationToken);
}