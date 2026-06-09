using AcervoImobiliario.Application.Factories;
using AcervoImobiliario.Application.Services;
using AcervoImobiliario.Domain.Enums;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Factories;

public class PropertyFactoryTests
{
    private readonly PropertyFactory _sut = new(new TextNormalizer());

    [Fact]
    public void Create_ComEnderecoValido_DeveNormalizarCamposDeTexto()
    {
        // Act
        var property = _sut.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "  Centro  ",
            "  Rua das Flores ",
            "100",
            ComplementType.Apartment,
            "  Apto  12 ");

        // Assert
        property.NeighborhoodNormalized.Should().Be("centro");
        property.StreetNormalized.Should().Be("rua das flores");
        property.ComplementValueNormalized.Should().Be("apto 12");
    }

    [Fact]
    public void Create_ComComplementTypeNone_NaoDeveDefinirComplementValueNormalized()
    {
        // Act
        var property = _sut.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "Rua A",
            "100",
            ComplementType.None);

        // Assert
        property.ComplementValue.Should().BeNull();
        property.ComplementValueNormalized.Should().BeNull();
    }

    [Fact]
    public void UpdateAddress_ComNovosDados_DeveNormalizarCamposEAtualizarIsActive()
    {
        // Arrange
        var property = _sut.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "Rua A",
            "100",
            ComplementType.None);

        // Act
        _sut.UpdateAddress(
            property,
            "city-2",
            "Contagem",
            "  Savassi ",
            "  Rua Pernambuco ",
            "200",
            ComplementType.Apartment,
            "  Apto 3 ",
            "IDX-2",
            isActive: false);

        // Assert
        property.CityId.Should().Be("city-2");
        property.CityNameSnapshot.Should().Be("Contagem");
        property.NeighborhoodNormalized.Should().Be("savassi");
        property.StreetNormalized.Should().Be("rua pernambuco");
        property.ComplementValueNormalized.Should().Be("apto 3");
        property.IsActive.Should().BeFalse();
        property.UpdatedAt.Should().NotBeNull();
    }
}
