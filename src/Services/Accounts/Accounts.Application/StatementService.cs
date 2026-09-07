using Accounts.Application.Contracts;
using Accounts.Application.Exceptions;
using Accounts.Application.Mappings;
using Accounts.Application.Ports;

namespace Accounts.Application;

/// <summary>Implementa la consulta de estados de cuenta.</summary>
public sealed class StatementService(IStatementRepository repository) : IStatementService
{
    public async Task<StatementResponse> GetStatementAsync(
        string identification,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var clientId = await repository.GetClientIdByIdentificationAsync(
            identification.Trim(),
            cancellationToken);

        if (clientId is null)
        {
            throw new NotFoundException("Cliente no encontrado.");
        }

        var start = startDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var end = endDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
        var accounts = await repository.GetAccountsAsync(
            clientId.Value,
            start,
            end,
            cancellationToken);

        var statementAccounts = accounts
            .Select(account => new StatementAccount(
                account.AccountNumber,
                account.AccountType,
                account.Balance,
                account.IsActive,
                account.Transactions.Select(x => x.ToResponse()).ToArray()))
            .ToArray();

        return new StatementResponse(clientId.Value, startDate, endDate, statementAccounts);
    }
}