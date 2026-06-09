using AcervoImobiliario.Domain.Entities;
using AcervoImobiliario.Domain.Exceptions;
using FluentAssertions;

namespace AcervoImobiliario.Domain.UnitTests.Entities;

public class CityTests
{
    [Fact]
    public void Create_ComDadosValidos_DeveCriarCidadeAtiva()
    {
        // Arrange
        const string id = "city-1";
        const string name = "Belo Horizonte";
        const string nameNormalized = "belo horizonte";
        const string state = "mg";

        // Act
        var city = City.Create(id, name, nameNormalized, state);

        // Assert
        city.Id.Should().Be(id);
        city.Name.Should().Be(name);
        city.NameNormalized.Should().Be(nameNormalized);
        city.State.Should().Be("MG");
        city.IsActive.Should().BeTrue();
        city.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        city.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Update_ComDadosValidos_DeveAtualizarCamposEUpdatedAt()
    {
        // Arrange
        var city = City.Create("city-1", "Contagem", "contagem", "MG");
        var updatedAtBefore = city.UpdatedAt;

        // Act
        city.Update("Betim", "betim", "MG", isActive: false);

        // Assert
        city.Name.Should().Be("Betim");
        city.NameNormalized.Should().Be("betim");
        city.State.Should().Be("MG");
        city.IsActive.Should().BeFalse();
        city.UpdatedAt.Should().NotBeNull();
        city.UpdatedAt.Should().BeAfter(updatedAtBefore ?? DateTime.MinValue);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ComNomeInvalido_DeveLancarDomainException(string name)
    {
        // Arrange & Act
        var act = () => City.Create("city-1", name, "nome", "MG");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("O nome da cidade é obrigatório.");
    }

    [Fact]
    public void Deactivate_QuandoAtiva_DeveDesativarCidade()
    {
        // Arrange
        var city = City.Create("city-1", "Contagem", "contagem", "MG");

        // Act
        city.Deactivate();

        // Assert
        city.IsActive.Should().BeFalse();
        city.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Activate_QuandoInativa_DeveAtivarCidade()
    {
        // Arrange
        var city = City.Restore("city-1", "Contagem", "contagem", "MG", false, DateTime.UtcNow, null);

        // Act
        city.Activate();

        // Assert
        city.IsActive.Should().BeTrue();
        city.UpdatedAt.Should().NotBeNull();
    }
}
