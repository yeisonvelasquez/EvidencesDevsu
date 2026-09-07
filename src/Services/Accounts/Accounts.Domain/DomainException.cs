namespace Accounts.Domain;

/// <summary>Excepción para reglas invariantes del dominio.</summary>
public class DomainException(string message) : Exception(message);