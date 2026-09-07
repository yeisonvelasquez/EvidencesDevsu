namespace Accounts.Domain;

/// <summary>Excepción de negocio para fondos insuficientes.</summary>
public sealed class InsufficientBalanceException() : DomainException("Saldo no disponible");