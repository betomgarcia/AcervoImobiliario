using AcervoImobiliario.Application.Factories;
using AcervoImobiliario.Application.Services;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Factories;

public class CityFactoryTests
{
    private readonly CityFactory _sut = new(new TextNormalizer());

    [Fact]
    public void Create_ComNomeValido_DeveNormalizarNome()
    {
        // Act
        var city = _sut.Create("city-1", "  Belo   Horizonte ", "mg");

        // Assert
        city.Name.Should().Be("Belo   Horizonte");
        city.NameNormalized.Should().Be("belo horizonte");
        city.State.Should().Be("MG");
        city.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Update_ComNovosDados_DeveAtualizarNomeEstadoEIsActive()
    {
        // Arrange
        var city = _sut.Create("city-1", "Contagem", "MG");

        // Act
        _sut.Update(city, "  Betim ", "mg", isActive: false);

        // Assert
        city.Name.Should().Be("Betim");
        city.NameNormalized.Should().Be("betim");
        city.State.Should().Be("MG");
        city.IsActive.Should().BeFalse();
        city.UpdatedAt.Should().NotBeNull();
    }
}
