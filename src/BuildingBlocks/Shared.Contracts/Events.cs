namespace Shared.Contracts;

/// <summary>Evento publicado cuando cambia el estado de un cliente.</summary>
public sealed record ClientChangedEvent(
    Guid MessageId,
    Guid ClientId,
    string FullName,
    bool IsActive,
    DateTimeOffset OccurredAt);

/// <summary>Evento publicado cuando se registra un movimiento.</summary>
public sealed record TransactionRegisteredEvent(
    Guid MessageId,
    Guid AccountId,
    Guid ClientId,
    decimal Amount,
    decimal Balance,
    DateTimeOffset OccurredAt);