using Clients.Application.Contracts;
using Clients.Domain;

namespace Clients.Application.Mappings;

internal static class ClientMapping
{
    public static ClientResponse ToResponse(this Client client) => new(
        client.ClientId,
        client.FirstName,
        client.LastName,
        client.Gender,
        client.Age,
        client.Identification,
        client.Address,
        client.Phone,
        client.IsActive,
        client.CreatedAt);
}