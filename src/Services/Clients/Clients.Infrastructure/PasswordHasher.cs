using BCrypt.Net;
using Clients.Application.Ports;

namespace Clients.Infrastructure;

/// <summary>Hash BCrypt para no almacenar contraseñas en texto plano.</summary>
public sealed class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => string.IsNullOrWhiteSpace(password)
        ? throw new ArgumentException("La contraseña es obligatoria.", nameof(password))
        : BCrypt.Net.BCrypt.HashPassword(password);
}