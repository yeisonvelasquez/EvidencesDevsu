using Accounts.Domain;

namespace Accounts.UnitTests;

public sealed class AccountTests
{
    [Theory]
    [InlineData("Savings", "Savings")]
    [InlineData("Checking", "Checking")]
    [InlineData("ahorros", "Savings")]
    [InlineData("corriente", "Checking")]
    public void AccountType_ShouldNormalizeAllowedValues(string accountType, string expectedType)
    {
        var account = new Account("478758", accountType, 100, Guid.NewGuid());

        Assert.Equal(expectedType, account.AccountType);
    }

    [Fact]
    public void AccountType_ShouldRejectUnknownValue()
    {
        var exception = Assert.Throws<DomainException>(() => new Account("478758", "string", 100, Guid.NewGuid()));

        Assert.Equal("El tipo de cuenta debe ser Savings o Checking.", exception.Message);
    }

    [Fact]
    public void RegisterDeposit_ShouldUpdateBalanceAndSnapshot()
    {
        var account = new Account("478758", "Savings", 100, Guid.NewGuid());

        var transaction = account.RegisterTransaction(50, TransactionType.Deposit, DateTimeOffset.UtcNow);

        Assert.Equal(150, account.Balance);
        Assert.Equal(100, transaction.PreviousBalance);
        Assert.Equal(150, transaction.ResultingBalance);
    }

    [Fact]
    public void RegisterWithdrawalWithoutFunds_ShouldRejectTransaction()
    {
        var account = new Account("478758", "Savings", 100, Guid.NewGuid());

        var exception = Assert.Throws<InsufficientBalanceException>(() => account.RegisterTransaction(101, TransactionType.Withdrawal, DateTimeOffset.UtcNow));

        Assert.Equal("Saldo no disponible", exception.Message);
        Assert.Empty(account.Transactions);
    }
}