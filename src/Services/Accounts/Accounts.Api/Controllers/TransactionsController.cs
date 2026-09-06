using Accounts.Application.Contracts;
using Accounts.Application.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Accounts.Api.Controllers;

/// <summary>Registra y consulta movimientos inmutables.</summary>
[ApiController]
[Route("api/v1/movimientos")]
public sealed class TransactionsController(IAccountService service) : ControllerBase
{
    /// <summary>Registra un depósito o retiro con control de saldo e idempotencia.</summary>
    /// <remarks>Type debe ser Deposit(0) o Withdrawal(1). Amount siempre debe ser positivo internamente se realiza el débito o crédito en la cuenta.</remarks>
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