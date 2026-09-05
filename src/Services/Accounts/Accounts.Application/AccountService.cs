using Accounts.Domain;

namespace Accounts.Application;

/// <summary>Implementa las operaciones transaccionales de cuentas.</summary>
public sealed class AccountService(IAccountRepository repository) : IAccountService
{
    public async Task<IReadOnlyList<AccountResponse>> ListAccountsAsync(Guid? clientId, CancellationToken cancellationToken) =>
        (await repository.ListAsync(clientId, cancellationToken)).Select(x => x.ToResponse()).ToArray();

    public async Task<AccountResponse> GetAccountAsync(Guid id, CancellationToken cancellationToken) =>
        (await repository.GetAsync(id, cancellationToken) ?? throw new NotFoundException("Cuenta no encontrada.")).ToResponse();

    public async Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        if (await repository.ExistsByNumberAsync(request.AccountNumber, null, cancellationToken)) throw new ConflictException("El número de cuenta ya existe.");
        var account = new Account(request.AccountNumber, request.AccountType, request.InitialBalance, request.ClientId);
        await repository.AddAsync(account, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return account.ToResponse();
    }

    public async Task<AccountResponse> UpdateAccountAsync(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        var account = await repository.GetAsync(id, cancellationToken) ?? throw new NotFoundException("Cuenta no encontrada.");
        if (string.IsNullOrWhiteSpace(request.AccountType)) throw new DomainException("El tipo de cuenta es obligatorio.");
        account.SetDetails(request.AccountType, request.IsActive);
        await repository.SaveChangesAsync(cancellationToken);
        return account.ToResponse();
    }

    public async Task<TransactionResponse> RegisterTransactionAsync(Guid accountId, CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        var existing = !string.IsNullOrWhiteSpace(request.IdempotencyKey)
            ? await repository.GetTransactionByIdempotencyKeyAsync(request.IdempotencyKey, cancellationToken)
            : null;
        if (existing is not null) return existing.ToResponse();
        var account = await repository.GetAsync(accountId, cancellationToken) ?? throw new NotFoundException("Cuenta no encontrada.");
        if (account.ClientId != request.ClientId) throw new ConflictException("La cuenta no pertenece al cliente indicado.");
        var transaction = account.RegisterTransaction(request.Amount, request.Type, DateTimeOffset.UtcNow);
        transaction.SetIdempotencyKey(request.IdempotencyKey);
        await repository.AddTransactionAsync(transaction, request.IdempotencyKey, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return transaction.ToResponse();
    }

    public async Task<StatementResponse> GetStatementAsync(Guid clientId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        if (startDate > endDate) throw new ValidationException("La fecha inicial no puede ser posterior a la fecha final.");
        var accounts = await repository.GetStatementAsync(clientId, startDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), endDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc), cancellationToken);
        return new StatementResponse(clientId, startDate, endDate, accounts.Select(x => new StatementAccount(x.AccountNumber, x.AccountType, x.Balance, x.IsActive, x.Transactions.Select(t => t.ToResponse()).ToArray())).ToArray());
    }
}

/// <summary>Excepción de recurso inexistente.</summary>
public sealed class NotFoundException(string message) : Exception(message);
/// <summary>Excepción de conflicto con el estado actual.</summary>
public sealed class ConflictException(string message) : Exception(message);
/// <summary>Excepción de entrada inválida.</summary>
public sealed class ValidationException(string message) : Exception(message);