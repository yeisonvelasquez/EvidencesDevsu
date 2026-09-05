using Accounts.Application;
using Microsoft.AspNetCore.Mvc;

namespace Accounts.Api.Controllers;

/// <summary>Registra y consulta movimientos inmutables.</summary>
[ApiController]
[Route("api/v1/movimientos")]
public sealed class TransactionsController(IAccountService service) : ControllerBase
{
    /// <summary>Registra un depósito o retiro con control de saldo e idempotencia.</summary>
    [HttpPost("{accountId:guid}")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<TransactionResponse>> Create(Guid accountId, CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        var response = await service.RegisterTransactionAsync(accountId, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }
}