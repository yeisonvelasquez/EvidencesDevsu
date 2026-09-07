using Clients.Application.Contracts;
using Clients.Application.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Clients.Api.Controllers;

/// <summary>Administra clientes del sistema.</summary>
[ApiController]
[Route("api/v1/clientes")]
public sealed class ClientsController(IClientService service) : ControllerBase
{
    /// <summary>Obtiene todos los clientes.</summary>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClientResponse>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<ClientResponse>> GetAll(CancellationToken cancellationToken) => service.ListAsync(cancellationToken);

    /// <summary>Obtiene un cliente por su identificador interno.</summary>
    /// <remarks>Ejemplo: GET /api/v1/clientes/10000000-0000-0000-0000-000000000002.</remarks>
    /// <param name="clientId">Identificador GUID del cliente. Ejemplo: 10000000-0000-0000-0000-000000000002.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    [HttpGet("{clientId:guid}")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ClientResponse> Get(Guid clientId, CancellationToken cancellationToken) => service.GetAsync(clientId, cancellationToken);

    /// <summary>Crea un cliente y almacena su contraseña como hash.</summary>
    /// <remarks>
    /// Gender acepta M, F, Male, Female, Masculino o Femenino; se normaliza a M o F.
    /// Ejemplo de body:
    /// { "firstName": "Ana", "lastName": "Torres", "gender": "F", "age": 29, "identification": "0102030499", "address": "Av. Central", "phone": "0991112222", "password": "secret" }
    /// </remarks>
    /// <param name="request">Datos personales, documento, contacto y contraseña del cliente.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
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
    /// <remarks>
    /// Ejemplo de body: { "firstName": "Ana", "lastName": "Torres", "gender": "F", "age": 30, "identification": "0102030499", "address": "Av. Central", "phone": "0991112222", "isActive": true }.
    /// </remarks>
    /// <param name="clientId">Identificador GUID del cliente. Ejemplo: 10000000-0000-0000-0000-000000000002.</param>
    /// <param name="request">Datos actualizados del cliente. La contraseña no se modifica en este endpoint.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    [HttpPut("{clientId:guid}")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<ClientResponse> Update(Guid clientId, UpdateClientRequest request, CancellationToken cancellationToken) => service.UpdateAsync(clientId, request, cancellationToken);

    /// <summary>Elimina un cliente.</summary>
    /// <remarks>Ejemplo: DELETE /api/v1/clientes/10000000-0000-0000-0000-000000000002.</remarks>
    /// <param name="clientId">Identificador GUID del cliente que se eliminará.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    [HttpDelete("{clientId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid clientId, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(clientId, cancellationToken);
        return NoContent();
    }
}