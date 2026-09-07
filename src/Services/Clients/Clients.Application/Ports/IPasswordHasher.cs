namespace Clients.Application.Ports;

/// <summary>Puerto para almacenar contraseñas sin conservar texto plano.</summary>
public interface IPasswordHasher
{
    string Hash(string password);
}