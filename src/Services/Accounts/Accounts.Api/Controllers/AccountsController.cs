using Accounts.Application;
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

    /// <summary>Obtiene una cuenta.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<AccountResponse> Get(Guid id, CancellationToken cancellationToken) => service.GetAccountAsync(id, cancellationToken);

    /// <summary>Crea una cuenta con referencia lógica al cliente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AccountResponse>> Create(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var response = await service.CreateAccountAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
    }

    /// <summary>Actualiza los datos administrativos de una cuenta.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<AccountResponse> Update(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken) => service.UpdateAccountAsync(id, request, cancellationToken);

    /// <summary>Actualiza parcialmente los datos administrativos de una cuenta.</summary>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    public Task<AccountResponse> Patch(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken) => service.UpdateAccountAsync(id, request, cancellationToken);
}