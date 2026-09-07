using Accounts.Application.Ports;
using Accounts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure;

public sealed class StatementRepository(AccountsDbContext dbContext) : IStatementRepository
{
    public Task<Guid?> GetClientIdByIdentificationAsync(string identification, CancellationToken cancellationToken) => dbContext.ClientProjections.AsNoTracking().Where(x => x.Identification == identification).Select(x => (Guid?)x.ClientId).SingleOrDefaultAsync(cancellationToken);
    public async Task<IReadOnlyList<Account>> GetAccountsAsync(Guid clientId, DateTimeOffset start, DateTimeOffset end, CancellationToken cancellationToken) => await dbContext.Accounts.AsNoTracking().Include(x => x.Transactions.Where(t => t.OccurredAt >= start && t.OccurredAt <= end)).Where(x => x.ClientId == clientId).OrderBy(x => x.AccountNumber).ToListAsync(cancellationToken);
}