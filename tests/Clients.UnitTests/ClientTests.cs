using Clients.Domain;

namespace Clients.UnitTests;

public sealed class ClientTests
{
    [Fact]
    public void CreateClient_ShouldSetActiveStateAndFullName()
    {
        var client = new Client(Guid.NewGuid(), "Jose", "Lema", "M", 30, "0102030405", "Calle principal", "0999999999", "hashed-password");

        Assert.True(client.IsActive);
        Assert.Equal("Jose Lema", client.FullName);
        Assert.Equal("hashed-password", client.PasswordHash);
    }

    [Fact]
    public void UpdateClient_WithInvalidAge_ShouldRejectChange()
    {
        var client = new Client(Guid.NewGuid(), "Jose", "Lema", "M", 30, "0102030405", "Calle principal", "0999999999", "hash");

        var exception = Assert.Throws<DomainException>(() => client.Update("Jose", "Lema", "M", 131, "0102030405", "Calle principal", "0999999999", true));

        Assert.Equal("La edad debe estar entre 0 y 130 años.", exception.Message);
    }
}