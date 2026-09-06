using Clients.Application;
using Microsoft.AspNetCore.Mvc;

namespace Clients.Api.Controllers;

/// <summary>Administra clientes del sistema.</summary>
[ApiController]
[Route("api/v1/clientes")]
public sealed class ClientsController(IClientService service) : ControllerBase
{
    /// <summary>Obtiene todos los clientes.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClientResponse>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<ClientResponse>> GetAll(CancellationToken cancellationToken) => service.ListAsync(cancellationToken);

    /// <summary>Obtiene un cliente por su identificador interno.</summary>
    [HttpGet("{clientId:guid}")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ClientResponse> Get(Guid clientId, CancellationToken cancellationToken) => service.GetAsync(clientId, cancellationToken);

    /// <summary>Crea un cliente almacenando su contraseña como hash.</summary>
    /// <remarks>Gender debe ser M o F. También se aceptan Male, Female, Masculino o Femenino.</remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClientResponse>> Create(CreateClientRequest request, CancellationToken cancellationToken)
    {
        var response = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { clientId = response.ClientId }, response);
    }

    /// <summary>Actualiza un cliente existente.</summary>
    [HttpPut("{clientId:guid}")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<ClientResponse> Update(Guid clientId, UpdateClientRequest request, CancellationToken cancellationToken) => service.UpdateAsync(clientId, request, cancellationToken);

    /// <summary>Elimina un cliente.</summary>
    [HttpDelete("{clientId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid clientId, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(clientId, cancellationToken);
        return NoContent();
    }
}