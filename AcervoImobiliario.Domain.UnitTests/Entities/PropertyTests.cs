using AcervoImobiliario.Domain.Entities;
using AcervoImobiliario.Domain.Enums;
using AcervoImobiliario.Domain.Exceptions;
using FluentAssertions;

namespace AcervoImobiliario.Domain.UnitTests.Entities;

public class PropertyTests
{
    [Fact]
    public void Create_ComEnderecoValido_DeveCriarImovelAtivo()
    {
        // Arrange
        const string id = "property-1";

        // Act
        var property = CreateValidProperty(id);

        // Assert
        property.Id.Should().Be(id);
        property.Number.Should().Be("100");
        property.ComplementType.Should().Be(ComplementType.Apartment);
        property.ComplementValue.Should().Be("101");
        property.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("10A")]
    [InlineData("S/N")]
    public void Create_ComNumeroInvalido_DeveLancarDomainException(string number)
    {
        // Arrange & Act
        var act = () => Property.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua A",
            "rua a",
            number,
            ComplementType.None);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("O número do imóvel deve conter somente dígitos.");
    }

    [Fact]
    public void Create_ComApartamentoSemComplementValue_DeveLancarDomainException()
    {
        // Arrange & Act
        var act = () => Property.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua A",
            "rua a",
            "100",
            ComplementType.Apartment,
            complementValue: null,
            complementValueNormalized: null);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage($"ComplementValue é obrigatório para o tipo de complemento '{ComplementType.Apartment}'.");
    }

    [Fact]
    public void Create_ComComplementTypeNoneEComplementValue_DeveLancarDomainException()
    {
        // Arrange & Act
        var act = () => Property.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua A",
            "rua a",
            "100",
            ComplementType.None,
            complementValue: "101",
            complementValueNormalized: "101");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Imóveis sem complemento não devem informar ComplementValue.");
    }

    [Fact]
    public void UpdateAddress_ComDadosValidos_DeveAtualizarEnderecoIsActiveEUpdatedAt()
    {
        // Arrange
        var property = CreateValidProperty("property-1");

        // Act
        property.UpdateAddress(
            "city-2",
            "Contagem",
            "Savassi",
            "savassi",
            "Rua Pernambuco",
            "rua pernambuco",
            "200",
            ComplementType.None,
            null,
            null,
            "IDX-2",
            isActive: false);

        // Assert
        property.CityId.Should().Be("city-2");
        property.CityNameSnapshot.Should().Be("Contagem");
        property.Number.Should().Be("200");
        property.IsActive.Should().BeFalse();
        property.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void GenerateAddressKey_DeveMontarChaveComCamposNormalizados()
    {
        // Arrange
        var property = CreateValidProperty("property-1");

        // Act
        var addressKey = property.GenerateAddressKey();

        // Assert
        addressKey.Should().Be("city-1|centro|rua a|100|Apartment|101");
    }

    private static Property CreateValidProperty(string id) =>
        Property.Create(
            id,
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua A",
            "rua a",
            "100",
            ComplementType.Apartment,
            "101",
            "101");
}
