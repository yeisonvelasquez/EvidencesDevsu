namespace Clients.Application.Contracts;

/// <summary>Datos para crear un cliente.</summary>
public sealed record CreateClientRequest(
    string FirstName,
    string LastName,
    string Gender,
    int Age,
    string Identification,
    string Address,
    string Phone,
    string Password);

/// <summary>Datos para actualizar un cliente.</summary>
public sealed record UpdateClientRequest(
    string FirstName,
    string LastName,
    string Gender,
    int Age,
    string Identification,
    string Address,
    string Phone,
    bool IsActive);