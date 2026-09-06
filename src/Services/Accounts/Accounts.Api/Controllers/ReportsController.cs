using Accounts.Application;
using Microsoft.AspNetCore.Mvc;

namespace Accounts.Api.Controllers;

/// <summary>Consulta estados de cuenta por cliente y rango de fechas.</summary>
[ApiController]
[Route("api/v1/reportes")]
public sealed class ReportsController(IAccountService service) : ControllerBase
{
    /// <summary>
    /// Retorna cuentas y movimientos del período indicado para una identificación.
    /// </summary>
    /// <param name="identificacion">Número de documento de identidad del cliente</param>
    /// <param name="fechaInicio">Fecha inicial del período a consultar en formato YYYY-MM-DD.</param>
    /// <param name="fechaFin">Fecha final del período a consultar en formato YYYY-MM-DD.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>Lista de las transacciones asociadas a las cuentas del cliente consultado</returns>
    [HttpGet]
    [ProducesResponseType(typeof(StatementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public Task<StatementResponse> Get(string identificacion, DateOnly fechaInicio, DateOnly fechaFin, CancellationToken cancellationToken) => service.GetStatementAsync(identificacion, fechaInicio, fechaFin, cancellationToken);
}