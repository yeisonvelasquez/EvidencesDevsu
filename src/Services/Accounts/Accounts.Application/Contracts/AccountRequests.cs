using Accounts.Domain;

namespace Accounts.Application.Contracts;

/// <summary>Datos para crear una cuenta.</summary>
public sealed record CreateAccountRequest(
    string AccountNumber,
    string AccountType,
    decimal InitialBalance,
    Guid ClientId);

/// <summary>Datos para actualizar una cuenta.</summary>
public sealed record UpdateAccountRequest(
    string AccountType,
    bool IsActive);

/// <summary>Datos para registrar un movimiento.</summary>
public sealed record CreateTransactionRequest(
    decimal Amount,
    TransactionType Type,
    string? IdempotencyKey);