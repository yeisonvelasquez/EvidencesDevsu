using Accounts.Application.Ports;
using Accounts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure;

public sealed class AccountCommandRepository(AccountsDbContext dbContext) : IAccountCommandRepository
{
    public Task<bool> IsClientActiveAsync(Guid clientId, CancellationToken cancellationToken) => dbContext.ClientProjections.AnyAsync(x => x.ClientId == clientId && x.IsActive, cancellationToken);
    public Task<Account?> GetAsync(Guid id, CancellationToken cancellationToken) => dbContext.Accounts.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    public Task<bool> ExistsByNumberAsync(string accountNumber, Guid? excludingId, CancellationToken cancellationToken) => dbContext.Accounts.AnyAsync(x => x.AccountNumber == accountNumber && (excludingId == null || x.Id != excludingId), cancellationToken);
    public Task AddAsync(Account account, CancellationToken cancellationToken) => dbContext.Accounts.AddAsync(account, cancellationToken).AsTask();
    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}