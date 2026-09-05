using Accounts.Application;
using Microsoft.AspNetCore.Mvc;

namespace Accounts.Api.Controllers;

/// <summary>Consulta estados de cuenta por cliente y rango de fechas.</summary>
[ApiController]
[Route("api/v1/reportes")]
public sealed class ReportsController(IAccountService service) : ControllerBase
{
    /// <summary>Retorna cuentas y movimientos del período indicado.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(StatementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public Task<StatementResponse> Get(Guid cliente, DateOnly fechaInicio, DateOnly fechaFin, CancellationToken cancellationToken) => service.GetStatementAsync(cliente, fechaInicio, fechaFin, cancellationToken);
}