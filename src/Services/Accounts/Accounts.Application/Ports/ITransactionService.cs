using Accounts.Application.Contracts;

namespace Accounts.Application.Ports;

/// <summary>Casos de uso para registrar movimientos de cuentas.</summary>
public interface ITransactionService
{
    Task<TransactionResponse> RegisterTransactionAsync(
        string accountNumber,
        CreateTransactionRequest request,
        CancellationToken cancellationToken);
}