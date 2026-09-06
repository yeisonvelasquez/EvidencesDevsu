using Accounts.Domain;

namespace Accounts.Application.Contracts;

/// <summary>Representación pública de una cuenta.</summary>
public sealed record AccountResponse(
    Guid Id,
    string AccountNumber,
    string AccountType,
    decimal Balance,
    bool IsActive,
    Guid ClientId,
    string? ClientName);

/// <summary>Representación pública de un movimiento.</summary>
public sealed record TransactionResponse(
    Guid Id,
    Guid AccountId,
    TransactionType Type,
    decimal Amount,
    decimal PreviousBalance,
    decimal ResultingBalance,
    DateTimeOffset OccurredAt);

/// <summary>Representación del estado de cuenta por cliente.</summary>
public sealed record StatementResponse(
    Guid ClientId,
    DateOnly StartDate,
    DateOnly EndDate,
    IReadOnlyList<StatementAccount> Accounts);

/// <summary>Cuenta incluida en un estado de cuenta.</summary>
public sealed record StatementAccount(
    string AccountNumber,
    string AccountType,
    decimal CurrentBalance,
    bool IsActive,
    IReadOnlyList<TransactionResponse> Transactions);
