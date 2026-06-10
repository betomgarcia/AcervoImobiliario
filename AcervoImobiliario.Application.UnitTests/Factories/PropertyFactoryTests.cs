using AcervoImobiliario.Application.Factories;
using AcervoImobiliario.Application.Services;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Factories;

public class PropertyFactoryTests
{
    private readonly PropertyFactory _sut = new(new TextNormalizer(), new AddressNormalizationService());

    [Fact]
    public void Create_ComEnderecoValido_DeveNormalizarCamposDeTexto()
    {
        var property = _sut.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "  Centro  ",
            "  Rua das Flores ",
            "100",
            "  Apartamento  303 Bloco A ");

        property.NeighborhoodNormalized.Should().Be("centro");
        property.StreetNormalized.Should().Be("rua das flores");
        property.Complement.Should().Be("Apartamento  303 Bloco A");
        property.ComplementNormalized.Should().Be("APT 303 BLOCO A");
    }

    [Fact]
    public void Create_SemComplemento_NaoDeveDefinirComplement()
    {
        var property = _sut.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "Rua A",
            "100");

        property.Complement.Should().BeNull();
        property.ComplementNormalized.Should().BeEmpty();
    }

    [Fact]
    public void UpdateAddress_ComNovosDados_DeveNormalizarCamposEAtualizarIsActive()
    {
        var property = _sut.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "Rua A",
            "100");

        _sut.UpdateAddress(
            property,
            "city-2",
            "Contagem",
            "  Savassi ",
            "  Rua Pernambuco ",
            "200",
            "  Apto 3 ",
            "IDX-2",
            isActive: false);

        property.CityId.Should().Be("city-2");
        property.CityNameSnapshot.Should().Be("Contagem");
        property.NeighborhoodNormalized.Should().Be("savassi");
        property.StreetNormalized.Should().Be("rua pernambuco");
        property.ComplementNormalized.Should().Be("APT 3");
        property.IsActive.Should().BeFalse();
        property.UpdatedAt.Should().NotBeNull();
    }
}
