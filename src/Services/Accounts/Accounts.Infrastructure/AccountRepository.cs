using Accounts.Application;
using Accounts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure;

/// <summary>Repositorio EF Core para cuentas y movimientos inmutables.</summary>
public sealed class AccountRepository(AccountsDbContext dbContext) : IAccountRepository
{
    public Task<bool> IsClientActiveAsync(Guid clientId, CancellationToken cancellationToken) => dbContext.ClientProjections.AnyAsync(x => x.ClientId == clientId && x.IsActive, cancellationToken);
    public async Task<IReadOnlyList<Account>> ListAsync(Guid? clientId, CancellationToken cancellationToken) => await dbContext.Accounts.AsNoTracking().Where(x => clientId == null || x.ClientId == clientId).OrderBy(x => x.AccountNumber).ToListAsync(cancellationToken);
    public Task<Account?> GetAsync(Guid id, CancellationToken cancellationToken) => dbContext.Accounts.Include(x => x.Transactions).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    public Task<bool> ExistsByNumberAsync(string accountNumber, Guid? excludingId, CancellationToken cancellationToken) => dbContext.Accounts.AnyAsync(x => x.AccountNumber == accountNumber && (excludingId == null || x.Id != excludingId), cancellationToken);
    public Task AddAsync(Account account, CancellationToken cancellationToken) => dbContext.Accounts.AddAsync(account, cancellationToken).AsTask();
    public async Task<IReadOnlyList<Account>> GetStatementAsync(Guid clientId, DateTimeOffset start, DateTimeOffset end, CancellationToken cancellationToken) => await dbContext.Accounts.AsNoTracking().Include(x => x.Transactions.Where(t => t.OccurredAt >= start && t.OccurredAt <= end)).Where(x => x.ClientId == clientId).OrderBy(x => x.AccountNumber).ToListAsync(cancellationToken);
    public Task<Transaction?> GetTransactionByIdempotencyKeyAsync(string key, CancellationToken cancellationToken) => dbContext.Transactions.SingleOrDefaultAsync(x => EF.Property<string?>(x, "IdempotencyKey") == key, cancellationToken);
    public Task AddTransactionAsync(Transaction transaction, string? idempotencyKey, CancellationToken cancellationToken)
    {
        dbContext.Entry(transaction).Property("IdempotencyKey").CurrentValue = idempotencyKey;
        return dbContext.Transactions.AddAsync(transaction, cancellationToken).AsTask();
    }
    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}