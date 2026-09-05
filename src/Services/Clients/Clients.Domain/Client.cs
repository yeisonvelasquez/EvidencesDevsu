namespace Clients.Domain;

/// <summary>Cliente que puede tener cuentas en el sistema bancario.</summary>
public sealed class Client : Person
{
    private Client() { }

    /// <summary>Crea un cliente nuevo con sus reglas básicas de dominio.</summary>
    public Client(string firstName, string lastName, string gender, int age, string identification, string address, string phone, string clientId, string passwordHash, Guid? id = null)
        : base(firstName, lastName, gender, age, identification, address, phone)
    {
        Id = id ?? Id;
        ClientId = Require(clientId, nameof(clientId));
        PasswordHash = Require(passwordHash, nameof(passwordHash));
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public string ClientId { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    /// <summary>Actualiza los datos editables del cliente.</summary>
    public void Update(string firstName, string lastName, string gender, int age, string identification, string address, string phone, bool isActive)
    {
        FirstName = string.IsNullOrWhiteSpace(firstName) ? throw new DomainException("El nombre es obligatorio.") : firstName.Trim();
        LastName = string.IsNullOrWhiteSpace(lastName) ? throw new DomainException("El apellido es obligatorio.") : lastName.Trim();
        Gender = string.IsNullOrWhiteSpace(gender) ? throw new DomainException("El género es obligatorio.") : gender.Trim();
        Identification = string.IsNullOrWhiteSpace(identification) ? throw new DomainException("La identificación es obligatoria.") : identification.Trim();
        Address = string.IsNullOrWhiteSpace(address) ? throw new DomainException("La dirección es obligatoria.") : address.Trim();
        Phone = string.IsNullOrWhiteSpace(phone) ? throw new DomainException("El teléfono es obligatorio.") : phone.Trim();
        if (age is < 0 or > 130) throw new DomainException("La edad debe estar entre 0 y 130 años.");
        Age = age;
        IsActive = isActive;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static string Require(string value, string name) => string.IsNullOrWhiteSpace(value)
        ? throw new DomainException($"{name} es obligatorio.")
        : value.Trim();
}