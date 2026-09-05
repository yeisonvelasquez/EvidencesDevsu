using Clients.Domain;

namespace Clients.Application;

/// <summary>Datos para crear un cliente.</summary>
public sealed record CreateClientRequest(string FirstName, string LastName, string Gender, int Age, string Identification, string Address, string Phone, string Password);

/// <summary>Datos para actualizar un cliente.</summary>
public sealed record UpdateClientRequest(string FirstName, string LastName, string Gender, int Age, string Identification, string Address, string Phone, bool IsActive);

/// <summary>Representación pública de un cliente.</summary>
public sealed record ClientResponse(Guid ClientId, string FirstName, string LastName, string Gender, int Age, string Identification, string Address, string Phone, bool IsActive, DateTimeOffset CreatedAt);

/// <summary>Puerto de persistencia de clientes.</summary>
public interface IClientRepository
{
    Task<IReadOnlyList<Client>> ListAsync(CancellationToken cancellationToken);
    Task<Client?> GetAsync(Guid clientId, CancellationToken cancellationToken);
    Task<bool> ExistsByIdentificationAsync(string identification, Guid? excludingClientId, CancellationToken cancellationToken);
    Task AddAsync(Client client, CancellationToken cancellationToken);
    void Remove(Client client);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

/// <summary>Servicio de aplicación para el ciclo de vida del cliente.</summary>
public interface IClientService
{
    Task<IReadOnlyList<ClientResponse>> ListAsync(CancellationToken cancellationToken);
    Task<ClientResponse> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<ClientResponse> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken);
    Task<ClientResponse> UpdateAsync(Guid id, UpdateClientRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}

/// <summary>Adaptador para almacenar contraseñas sin conservar texto plano.</summary>
public interface IPasswordHasher
{
    string Hash(string password);
}

/// <summary>Almacena eventos de integración junto con el cambio de dominio.</summary>
public interface IClientEventStore
{
    Task EnqueueAsync(Client client, CancellationToken cancellationToken);
}

internal static class ClientMapping
{
    public static ClientResponse ToResponse(this Client client) => new(client.ClientId, client.FirstName, client.LastName, client.Gender, client.Age, client.Identification, client.Address, client.Phone, client.IsActive, client.CreatedAt);
}