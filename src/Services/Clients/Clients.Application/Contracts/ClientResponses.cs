namespace Clients.Application.Contracts;

/// <summary>Representación pública de un cliente.</summary>
public sealed record ClientResponse(
    Guid ClientId,
    string FirstName,
    string LastName,
    string Gender,
    int Age,
    string Identification,
    string Address,
    string Phone,
    bool IsActive,
    DateTimeOffset CreatedAt);