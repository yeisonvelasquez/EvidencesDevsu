using Accounts.Application.Contracts;
using Accounts.Application.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Accounts.Api.Controllers;

/// <summary>Consulta estados de cuenta por cliente y rango de fechas.</summary>
[ApiController]
[Route("api/v1/reportes")]
public sealed class ReportsController(IStatementService service) : ControllerBase
{
    /// <summary>
    /// Retorna cuentas y movimientos del período indicado para una identificación.
    /// </summary>
    /// <remarks>
    /// La consulta recibe el número de documento, no el GUID interno del cliente.
    /// Las fechas deben usar el formato ISO 8601 YYYY-MM-DD.
    /// Ejemplo: GET /api/v1/reportes?identificacion=0102030406&amp;fechaInicio=2022-02-01&amp;fechaFin=2022-02-28.
    /// </remarks>
    /// <param name="request">Ejemplo: identificacion=0102030406, fechaInicio=2022-02-01, fechaFin=2022-02-28.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>Lista de las transacciones asociadas a las cuentas del cliente consultado</returns>
    [HttpGet]
    [ProducesResponseType(typeof(StatementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public Task<StatementResponse> Get([FromQuery] StatementRequest request, CancellationToken cancellationToken) => service.GetStatementAsync(request.Identificacion, request.FechaInicio, request.FechaFin, cancellationToken);
}