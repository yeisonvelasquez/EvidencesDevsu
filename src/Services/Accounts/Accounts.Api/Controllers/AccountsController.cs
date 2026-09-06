using Accounts.Application.Contracts;
using Accounts.Application.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Accounts.Api.Controllers;

/// <summary>Administra cuentas y sus movimientos.</summary>
[ApiController]
[Route("api/v1/cuentas")]
public sealed class AccountsController(IAccountService service) : ControllerBase
{
    /// <summary>Obtiene cuentas, opcionalmente filtradas por cliente.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AccountResponse>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<AccountResponse>> GetAll(Guid? clientId, CancellationToken cancellationToken) => service.ListAccountsAsync(clientId, cancellationToken);

    /// <summary>Obtiene una cuenta por numero de cuenta.</summary>
    [HttpGet("{accountNumber}")]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<AccountResponse> Get(string accountNumber, CancellationToken cancellationToken) => service.GetAccountByNumberAsync(accountNumber, cancellationToken);

    /// <summary>Crea una cuenta con referencia lógica al cliente.</summary>
    /// <remarks>AccountType debe ser Savings o Checking. También se aceptan Ahorros o Corriente.</remarks>
    [HttpPost]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AccountResponse>> Create(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var response = await service.CreateAccountAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { accountNumber = response.AccountNumber }, response);
    }

    /// <summary>Actualiza los datos administrativos de una cuenta.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<AccountResponse> Update(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken) => service.UpdateAccountAsync(id, request, cancellationToken);

}