namespace Accounts.Domain;

/// <summary>Movimiento inmutable que registra una operación sobre una cuenta.</summary>
public sealed class Transaction
{
    private Transaction() { }

    internal Transaction(Guid accountId, TransactionType type, decimal amount, decimal previousBalance, decimal resultingBalance, DateTimeOffset occurredAt)
    {
        Id = Guid.NewGuid();
        AccountId = accountId;
        Type = type;
        Amount = amount;
        PreviousBalance = previousBalance;
        ResultingBalance = resultingBalance;
        OccurredAt = occurredAt;
    }

    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public decimal PreviousBalance { get; private set; }
    public decimal ResultingBalance { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string? IdempotencyKey { get; private set; }

    /// <summary>Asocia una clave de idempotencia al registro antes de persistirlo.</summary>
    public void SetIdempotencyKey(string? idempotencyKey) => IdempotencyKey = string.IsNullOrWhiteSpace(idempotencyKey) ? null : idempotencyKey.Trim();
}