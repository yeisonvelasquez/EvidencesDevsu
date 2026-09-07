using Clients.Application.Contracts;
using Clients.Application.Exceptions;
using Clients.Application.Mappings;
using Clients.Application.Ports;
using Clients.Domain;

namespace Clients.Application;

/// <summary>Implementa los casos de uso de clientes y sus validaciones.</summary>
public sealed class ClientService(IClientRepository repository, IPasswordHasher passwordHasher, IClientEventStore eventStore) : IClientService
{
    public async Task<IReadOnlyList<ClientResponse>> ListAsync(CancellationToken cancellationToken) =>
        (await repository.ListAsync(cancellationToken)).Select(x => x.ToResponse()).ToArray();

    public async Task<ClientResponse> GetAsync(Guid id, CancellationToken cancellationToken) =>
        (await repository.GetAsync(id, cancellationToken) ?? throw new NotFoundException("Cliente no encontrado.")).ToResponse();

    public async Task<ClientResponse> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken)
    {
        if (await repository.ExistsByIdentificationAsync(request.Identification, null, cancellationToken)) throw new ConflictException("La identificación ya existe.");
        var client = new Client(Guid.NewGuid(), request.FirstName, request.LastName, request.Gender, request.Age, request.Identification, request.Address, request.Phone, passwordHasher.Hash(request.Password));
        await repository.AddAsync(client, cancellationToken);
        await eventStore.EnqueueAsync(client, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return client.ToResponse();
    }

    public async Task<ClientResponse> UpdateAsync(Guid id, UpdateClientRequest request, CancellationToken cancellationToken)
    {
        var client = await repository.GetAsync(id, cancellationToken) ?? throw new NotFoundException("Cliente no encontrado.");
        if (await repository.ExistsByIdentificationAsync(request.Identification, id, cancellationToken)) throw new ConflictException("La identificación ya existe.");
        client.Update(request.FirstName, request.LastName, request.Gender, request.Age, request.Identification, request.Address, request.Phone, request.IsActive);
        await eventStore.EnqueueAsync(client, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return client.ToResponse();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var client = await repository.GetAsync(id, cancellationToken) ?? throw new NotFoundException("Cliente no encontrado.");
        repository.Remove(client);
        await repository.SaveChangesAsync(cancellationToken);
    }
}

