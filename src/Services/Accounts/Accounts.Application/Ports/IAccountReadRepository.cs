using Accounts.Application.ReadModels;

namespace Accounts.Application.Ports;

public interface IAccountReadRepository
{
    Task<IReadOnlyList<AccountReadModel>> ListAsync(Guid? clientId, CancellationToken cancellationToken);
    Task<AccountReadModel?> GetByNumberAsync(string accountNumber, CancellationToken cancellationToken);
}