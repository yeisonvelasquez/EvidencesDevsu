using Accounts.Application;
using Accounts.Application.Contracts;
using Accounts.Domain;
using Accounts.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests;

public sealed class AccountPersistenceTests
{
    [Fact]
    public async Task RegisterTransaction_ShouldPersistBalanceAndTransactionTogether()
    {
        var options = new DbContextOptionsBuilder<AccountsDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new AccountsDbContext(options);
        db.Database.EnsureCreated();
        var account = new Account("225487", "Checking", 100, Guid.NewGuid());
        db.ClientProjections.Add(new ClientProjection { ClientId = account.ClientId, FullName = "Test Client", IsActive = true, UpdatedAt = DateTimeOffset.UtcNow });
        db.Accounts.Add(account);
        await db.SaveChangesAsync();
        var service = new AccountService(new AccountRepository(db));

        var result = await service.RegisterTransactionAsync("225487", new CreateTransactionRequest(600, TransactionType.Deposit, "integration-1"), CancellationToken.None);
        var retry  = await service.RegisterTransactionAsync("225487", new CreateTransactionRequest(600, TransactionType.Deposit, "integration-1"), CancellationToken.None);
        var stored = await db.Accounts.Include(x => x.Transactions).SingleAsync();

        Assert.Equal(result.Id, retry.Id);
        Assert.Equal(700, result.ResultingBalance);
        Assert.Equal(700, stored.Balance);
        Assert.Single(stored.Transactions);
    }
}