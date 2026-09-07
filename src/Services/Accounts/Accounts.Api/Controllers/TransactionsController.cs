using Accounts.Application.Contracts;
using Accounts.Application.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Accounts.Api.Controllers;

/// <summary>Registra y consulta movimientos inmutables.</summary>
[ApiController]
[Route("api/v1/movimientos")]
public sealed class TransactionsController(ITransactionService service) : ControllerBase
{
    /// <summary>Registra un depósito o retiro sobre una cuenta.</summary>
    /// <remarks>
    /// Type acepta Deposit para acreditar o Withdrawal para debitar.
    /// Amount siempre debe ser positivo; el tipo determina si aumenta o disminuye el saldo.
    /// IdempotencyKey es opcional, pero se recomienda para reintentos.
    /// Ejemplo de body: { "amount": 100.00, "type": "Deposit", "idempotencyKey": "deposito-225487-001" }.
    /// </remarks>
    /// <param name="accountNumber">Número de cuenta sobre la que se aplicará el movimiento. Ejemplo: 225487.</param>
    /// <param name="request">Importe, tipo de movimiento y clave opcional de idempotencia.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    [HttpPost("{accountNumber}")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<TransactionResponse>> Create(string accountNumber, CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        var response = await service.RegisterTransactionAsync(accountNumber, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }
}