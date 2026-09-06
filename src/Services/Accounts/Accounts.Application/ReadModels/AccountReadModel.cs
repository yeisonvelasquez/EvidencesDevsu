namespace Accounts.Application.ReadModels;

/// <summary>
/// Modelo de lectura de una cuenta con información proyectada del cliente.
/// </summary>
public sealed record AccountReadModel(
    Guid Id,
    string AccountNumber,
    string AccountType,
    decimal Balance,
    bool IsActive,
    Guid ClientId,
    string? ClientName);