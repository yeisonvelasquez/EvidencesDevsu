namespace Accounts.Domain;

/// <summary>Cuenta bancaria y su saldo actual.</summary>
public sealed class Account
{
    private Account() { }

    /// <summary>Crea una cuenta asociada lógicamente a un cliente.</summary>
    public Account(string accountNumber, string accountType, decimal initialBalance, Guid clientId)
    {
        if (string.IsNullOrWhiteSpace(accountNumber)) throw new DomainException("El número de cuenta es obligatorio.");
        if (initialBalance < 0) throw new DomainException("El saldo inicial no puede ser negativo.");
        if (clientId == Guid.Empty) throw new DomainException("El cliente es obligatorio.");
        Id = Guid.NewGuid();
        AccountNumber = accountNumber.Trim();
        AccountType = NormalizeAccountType(accountType);
        Balance = initialBalance;
        ClientId = clientId;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public string AccountNumber { get; private set; } = string.Empty;
    public string AccountType { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }
    public bool IsActive { get; private set; }
    public Guid ClientId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public uint Version { get; private set; }
    public List<Transaction> Transactions { get; private set; } = [];

    /// <summary>Aplica un movimiento y devuelve el registro inmutable generado.</summary>
    public Transaction RegisterTransaction(decimal amount, TransactionType type, DateTimeOffset occurredAt)
    {
        if (!IsActive) throw new DomainException("La cuenta está inactiva.");
        if (amount <= 0) throw new DomainException("El valor del movimiento debe ser mayor que cero.");
        var delta = type == TransactionType.Deposit ? amount : -amount;
        var newBalance = Balance + delta;
        if (newBalance < 0) throw new InsufficientBalanceException();
        var transaction = new Transaction(Id, type, amount, Balance, newBalance, occurredAt);
        Balance = newBalance;
        Version++;
        UpdatedAt = DateTimeOffset.UtcNow;
        Transactions.Add(transaction);
        return transaction;
    }

    /// <summary>Actualiza únicamente los datos administrativos de la cuenta.</summary>
    public void SetDetails(string accountType, bool isActive)
    {
        AccountType = NormalizeAccountType(accountType);
        IsActive = isActive;
        Version++;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static string NormalizeAccountType(string accountType)
    {
        if (string.IsNullOrWhiteSpace(accountType)) throw new DomainException("El tipo de cuenta es obligatorio.");
        return accountType.Trim().ToLowerInvariant() switch
        {
            "savings" or "ahorros" => "Savings",
            "checking" or "corriente" => "Checking",
            _ => throw new DomainException("El tipo de cuenta debe ser Savings o Checking.")
        };
    }
}

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

/// <summary>Tipos de operación permitidos para una cuenta.</summary>
public enum TransactionType { Deposit, Withdrawal }

/// <summary>Excepción de negocio para fondos insuficientes.</summary>
public sealed class InsufficientBalanceException() : DomainException("Saldo no disponible");

/// <summary>Excepción para reglas invariantes de cuentas y movimientos.</summary>
public class DomainException(string message) : Exception(message);