using Accounts.Application.Ports;
using Accounts.Application.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure;

public sealed class AccountReadRepository(AccountsDbContext dbContext) : IAccountReadRepository
{
    public async Task<IReadOnlyList<AccountReadModel>> ListAsync(Guid? clientId, CancellationToken cancellationToken) => await (
        from account in dbContext.Accounts.AsNoTracking()
        join client in dbContext.ClientProjections.AsNoTracking() on account.ClientId equals client.ClientId into clients
        from client in clients.DefaultIfEmpty()
        where clientId == null || account.ClientId == clientId
        orderby account.AccountNumber
        select new AccountReadModel(account.Id, account.AccountNumber, account.AccountType, account.Balance, account.IsActive, account.ClientId, client == null ? null : client.FullName))
        .ToListAsync(cancellationToken);

    public Task<AccountReadModel?> GetByNumberAsync(string accountNumber, CancellationToken cancellationToken) => (
        from account in dbContext.Accounts.AsNoTracking()
        join client in dbContext.ClientProjections.AsNoTracking() on account.ClientId equals client.ClientId into clients
        from client in clients.DefaultIfEmpty()
        where account.AccountNumber == accountNumber
        select new AccountReadModel(account.Id, account.AccountNumber, account.AccountType, account.Balance, account.IsActive, account.ClientId, client == null ? null : client.FullName))
        .SingleOrDefaultAsync(cancellationToken);
}