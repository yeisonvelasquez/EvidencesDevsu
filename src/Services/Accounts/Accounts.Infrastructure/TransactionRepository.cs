using Accounts.Application.Ports;
using Accounts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure;

public sealed class TransactionRepository(AccountsDbContext dbContext) : ITransactionRepository
{
    public Task<Account?> GetAccountForUpdateAsync(string accountNumber, CancellationToken cancellationToken) => dbContext.Accounts.SingleOrDefaultAsync(x => x.AccountNumber == accountNumber, cancellationToken);
    public Task<bool> IsClientActiveAsync(Guid clientId, CancellationToken cancellationToken) => dbContext.ClientProjections.AnyAsync(x => x.ClientId == clientId && x.IsActive, cancellationToken);
    public Task<Transaction?> GetByIdempotencyKeyAsync(string key, CancellationToken cancellationToken) => dbContext.Transactions.AsNoTracking().SingleOrDefaultAsync(x => EF.Property<string?>(x, "IdempotencyKey") == key, cancellationToken);
    public Task AddAsync(Transaction transaction, string? idempotencyKey, CancellationToken cancellationToken)
    {
        dbContext.Entry(transaction).Property("IdempotencyKey").CurrentValue = idempotencyKey;
        return dbContext.Transactions.AddAsync(transaction, cancellationToken).AsTask();
    }
    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}