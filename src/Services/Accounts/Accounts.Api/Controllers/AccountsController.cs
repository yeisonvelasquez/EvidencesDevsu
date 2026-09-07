using Accounts.Application.Contracts;
using Accounts.Application.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Accounts.Api.Controllers;

/// <summary>Administra cuentas y sus movimientos.</summary>
[ApiController]
[Route("api/v1/cuentas")]
public sealed class AccountsController(IAccountService service) : ControllerBase
{
    /// <summary>Obtiene todas las cuentas o las filtra por el identificador del cliente.</summary>
    /// <remarks>
    /// Ejemplo sin filtro: GET /api/v1/cuentas.
    /// Ejemplo filtrado: GET /api/v1/cuentas?clientId=10000000-0000-0000-0000-000000000002.
    /// </remarks>
    /// <param name="clientId">Identificador GUID opcional del cliente. Ejemplo: 10000000-0000-0000-0000-000000000002.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AccountResponse>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<AccountResponse>> GetAll(Guid? clientId, CancellationToken cancellationToken) => service.ListAccountsAsync(clientId, cancellationToken);

    /// <summary>Obtiene una cuenta por número de cuenta.</summary>
    /// <remarks>Ejemplo: GET /api/v1/cuentas/225487.</remarks>
    /// <param name="accountNumber">Número de cuenta. Ejemplo: 225487.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    [HttpGet("{accountNumber}")]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<AccountResponse> Get(string accountNumber, CancellationToken cancellationToken) => service.GetAccountByNumberAsync(accountNumber, cancellationToken);

    /// <summary>Crea una cuenta asociada a un cliente.</summary>
    /// <remarks>
    /// Valores permitidos para AccountType: Savings, Ahorros, Checking o Corriente.
    /// El valor se normaliza a Savings o Checking.
    /// Ejemplo de body:
    /// { "accountNumber": "123456", "accountType": "Savings", "initialBalance": 1500.00, "clientId": "10000000-0000-0000-0000-000000000002" }
    /// </remarks>
    /// <param name="request">Datos de la cuenta: número, tipo, saldo inicial y GUID del cliente.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    [HttpPost]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AccountResponse>> Create(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var response = await service.CreateAccountAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { accountNumber = response.AccountNumber }, response);
    }

    /// <summary>Actualiza los datos administrativos de una cuenta.</summary>
    /// <remarks>
    /// Solo se actualizan AccountType e IsActive.
    /// AccountType acepta Savings, Ahorros, Checking o Corriente.
    /// Ejemplo de body: { "accountType": "Checking", "isActive": true }.
    /// </remarks>
    /// <param name="id">Identificador GUID interno de la cuenta. Ejemplo: 9d6a8f65-6f8e-4cf2-9db6-7d8c7ce70f10.</param>
    /// <param name="request">Datos administrativos que se actualizarán.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<AccountResponse> Update(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken) => service.UpdateAccountAsync(id, request, cancellationToken);

}