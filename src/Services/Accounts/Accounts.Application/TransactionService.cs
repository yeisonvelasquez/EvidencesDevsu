using Accounts.Application.Contracts;
using Accounts.Application.Exceptions;
using Accounts.Application.Mappings;
using Accounts.Application.Ports;

namespace Accounts.Application;

/// <summary>Implementa el registro de movimientos de cuentas.</summary>
public sealed class TransactionService(ITransactionRepository repository) : ITransactionService
{
    public async Task<TransactionResponse> RegisterTransactionAsync(
        string accountNumber,
        CreateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var existingTransaction = string.IsNullOrWhiteSpace(request.IdempotencyKey)
            ? null
                : await repository.GetByIdempotencyKeyAsync(
                request.IdempotencyKey,
                cancellationToken);

        if (existingTransaction is not null)
        {
            return existingTransaction.ToResponse();
        }

        var account = await repository.GetAccountForUpdateAsync(
            accountNumber.Trim(),
            cancellationToken);

        if (account is null)
        {
            throw new NotFoundException("Cuenta no encontrada.");
        }

        if (!await repository.IsClientActiveAsync(account.ClientId, cancellationToken))
        {
            throw new ValidationException("El cliente no existe o está inactivo.");
        }

        var transaction = account.RegisterTransaction(
            request.Amount,
            request.Type,
            DateTimeOffset.UtcNow);

        transaction.SetIdempotencyKey(request.IdempotencyKey);
        await repository.AddAsync(
            transaction,
            request.IdempotencyKey,
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return transaction.ToResponse();
    }
}