namespace Clients.Domain;

/// <summary>Datos personales compartidos por las entidades del servicio.</summary>
public abstract class Person
{
    protected Person() { }

    protected Person(string firstName, string lastName, string gender, int age, string identification, string address, string phone)
    {
        Id = Guid.NewGuid();
        FirstName = Require(firstName, nameof(firstName));
        LastName = Require(lastName, nameof(lastName));
        Gender = Require(gender, nameof(gender));
        Identification = Require(identification, nameof(identification));
        Address = Require(address, nameof(address));
        Phone = Require(phone, nameof(phone));
        if (age is < 0 or > 130) throw new DomainException("La edad debe estar entre 0 y 130 años.");
        Age = age;
    }

    public Guid Id { get; protected set; }
    public string FirstName { get; protected set; } = string.Empty;
    public string LastName { get; protected set; } = string.Empty;
    public string Gender { get; protected set; } = string.Empty;
    public int Age { get; protected set; }
    public string Identification { get; protected set; } = string.Empty;
    public string Address { get; protected set; } = string.Empty;
    public string Phone { get; protected set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";

    private static string Require(string value, string name) => string.IsNullOrWhiteSpace(value)
        ? throw new DomainException($"{name} es obligatorio.")
        : value.Trim();
}

/// <summary>Excepción para reglas invariantes del dominio.</summary>
public class DomainException(string message) : Exception(message);