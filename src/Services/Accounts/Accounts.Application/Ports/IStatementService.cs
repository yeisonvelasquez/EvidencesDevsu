using Accounts.Application.Contracts;

namespace Accounts.Application.Ports;

/// <summary>Casos de uso para consultar estados de cuenta.</summary>
public interface IStatementService
{
    Task<StatementResponse> GetStatementAsync(
        string identification,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken);
}