namespace Accounts.Domain;

/// <summary>Cuenta bancaria y su saldo actual.</summary>
public sealed class Account
{
    public const string SavingsType = "Savings";
    public const string CheckingType = "Checking";

    private Account() { }
    private readonly List<Transaction> transactions = [];

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
    public IReadOnlyCollection<Transaction> Transactions => transactions.AsReadOnly();

    /// <summary>Aplica un movimiento y devuelve el registro inmutable generado.</summary>
    public Transaction RegisterTransaction(decimal amount, TransactionType type, DateTimeOffset occurredAt)
    {
        if (!IsActive) throw new DomainException("La cuenta está inactiva.");
        if (amount <= 0) throw new DomainException("El valor del movimiento debe ser mayor que cero.");
        if (occurredAt == default) throw new DomainException("La fecha del movimiento es obligatoria.");
        var delta = type switch
        {
            TransactionType.Deposit => amount,
            TransactionType.Withdrawal => -amount,
            _ => throw new DomainException("El tipo de movimiento no es válido.")
        };
        var newBalance = Balance + delta;
        if (newBalance < 0) throw new InsufficientBalanceException();
        var transaction = new Transaction(Id, type, amount, Balance, newBalance, occurredAt);
        Balance = newBalance;
        Version++;
        UpdatedAt = DateTimeOffset.UtcNow;
        transactions.Add(transaction);
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
            "savings" or "ahorros" => SavingsType,
            "checking" or "corriente" => CheckingType,
            _ => throw new DomainException($"El tipo de cuenta debe ser {SavingsType} o {CheckingType}.")
        };
    }
}

