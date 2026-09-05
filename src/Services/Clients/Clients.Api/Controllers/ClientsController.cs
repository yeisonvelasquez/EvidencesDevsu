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
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ClientResponse> Get(Guid id, CancellationToken cancellationToken) => service.GetAsync(id, cancellationToken);

    /// <summary>Crea un cliente almacenando su contraseña como hash.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClientResponse>> Create(CreateClientRequest request, CancellationToken cancellationToken)
    {
        var response = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
    }

    /// <summary>Actualiza un cliente existente.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<ClientResponse> Update(Guid id, UpdateClientRequest request, CancellationToken cancellationToken) => service.UpdateAsync(id, request, cancellationToken);

    /// <summary>Actualiza parcialmente un cliente usando el contrato validado.</summary>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ClientResponse> Patch(Guid id, UpdateClientRequest request, CancellationToken cancellationToken) => service.UpdateAsync(id, request, cancellationToken);

    /// <summary>Elimina un cliente.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}