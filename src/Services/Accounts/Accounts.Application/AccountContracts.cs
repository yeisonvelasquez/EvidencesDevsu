using Accounts.Domain;

namespace Accounts.Application;

/// <summary>Datos para crear una cuenta.</summary>
public sealed record CreateAccountRequest(string AccountNumber, string AccountType, decimal InitialBalance, Guid ClientId);

/// <summary>Datos para actualizar una cuenta.</summary>
public sealed record UpdateAccountRequest(string AccountType, bool IsActive);

/// <summary>Datos para registrar un movimiento.</summary>
public sealed record CreateTransactionRequest(decimal Amount, TransactionType Type, Guid ClientId, string? IdempotencyKey);

/// <summary>Representación pública de una cuenta.</summary>
public sealed record AccountResponse(Guid Id, string AccountNumber, string AccountType, decimal Balance, bool IsActive, Guid ClientId);

/// <summary>Representación pública de un movimiento.</summary>
public sealed record TransactionResponse(Guid Id, Guid AccountId, TransactionType Type, decimal Amount, decimal PreviousBalance, decimal ResultingBalance, DateTimeOffset OccurredAt);

/// <summary>Representación del estado de cuenta por cliente.</summary>
public sealed record StatementResponse(Guid ClientId, DateOnly StartDate, DateOnly EndDate, IReadOnlyList<StatementAccount> Accounts);
public sealed record StatementAccount(string AccountNumber, string AccountType, decimal CurrentBalance, bool IsActive, IReadOnlyList<TransactionResponse> Transactions);

/// <summary>Puerto de persistencia de cuentas.</summary>
public interface IAccountRepository
{
    Task<bool> IsClientActiveAsync(Guid clientId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Account>> ListAsync(Guid? clientId, CancellationToken cancellationToken);
    Task<Account?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByNumberAsync(string accountNumber, Guid? excludingId, CancellationToken cancellationToken);
    Task AddAsync(Account account, CancellationToken cancellationToken);
    Task<IReadOnlyList<Account>> GetStatementAsync(Guid clientId, DateTimeOffset start, DateTimeOffset end, CancellationToken cancellationToken);
    Task<Transaction?> GetTransactionByIdempotencyKeyAsync(string key, CancellationToken cancellationToken);
    Task AddTransactionAsync(Transaction transaction, string? idempotencyKey, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

/// <summary>Casos de uso de cuentas, movimientos y reportes.</summary>
public interface IAccountService
{
    Task<IReadOnlyList<AccountResponse>> ListAccountsAsync(Guid? clientId, CancellationToken cancellationToken);
    Task<AccountResponse> GetAccountAsync(Guid id, CancellationToken cancellationToken);
    Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request, CancellationToken cancellationToken);
    Task<AccountResponse> UpdateAccountAsync(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken);
    Task<TransactionResponse> RegisterTransactionAsync(Guid accountId, CreateTransactionRequest request, CancellationToken cancellationToken);
    Task<StatementResponse> GetStatementAsync(Guid clientId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
}

internal static class AccountMapping
{
    public static AccountResponse ToResponse(this Account x) => new(x.Id, x.AccountNumber, x.AccountType, x.Balance, x.IsActive, x.ClientId);
    public static TransactionResponse ToResponse(this Transaction x) => new(x.Id, x.AccountId, x.Type, x.Amount, x.PreviousBalance, x.ResultingBalance, x.OccurredAt);
}