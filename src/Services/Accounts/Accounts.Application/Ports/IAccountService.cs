using Accounts.Application.Contracts;

namespace Accounts.Application.Ports;

/// <summary>
/// Casos de uso de cuentas, movimientos y reportes.
/// </summary>
public interface IAccountService
{
    Task<IReadOnlyList<AccountResponse>> ListAccountsAsync(Guid? clientId, CancellationToken cancellationToken);

    Task<AccountResponse> GetAccountByNumberAsync(string accountNumber, CancellationToken cancellationToken);

    Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request, CancellationToken cancellationToken);

    Task<AccountResponse> UpdateAccountAsync(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken);

}
