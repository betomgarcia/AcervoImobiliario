using AcervoImobiliario.Domain.Entities;
using AcervoImobiliario.Domain.Exceptions;
using FluentAssertions;

namespace AcervoImobiliario.Domain.UnitTests.Entities;

public class PropertyTests
{
    [Fact]
    public void Create_ComEnderecoValido_DeveCriarImovelAtivo()
    {
        var property = CreateValidProperty("property-1");

        property.Id.Should().Be("property-1");
        property.Number.Should().Be("100");
        property.Complement.Should().Be("Apartamento 101");
        property.ComplementNormalized.Should().Be("APT 101");
        property.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("10A")]
    [InlineData("S/N")]
    public void Create_ComNumeroInvalido_DeveLancarDomainException(string number)
    {
        var act = () => Property.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua A",
            "rua a",
            number);

        act.Should().Throw<DomainException>()
            .WithMessage("O número do imóvel deve conter somente dígitos.");
    }

    [Fact]
    public void Create_ComComplementoSemNormalizado_DeveLancarDomainException()
    {
        var act = () => Property.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua A",
            "rua a",
            "100",
            "Apartamento 101",
            complementNormalized: string.Empty);

        act.Should().Throw<DomainException>()
            .WithMessage("ComplementNormalized é obrigatório quando Complement é informado.");
    }

    [Fact]
    public void Create_SemComplementoComNormalizado_DeveLancarDomainException()
    {
        var act = () => Property.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua A",
            "rua a",
            "100",
            complement: null,
            complementNormalized: "APT 101");

        act.Should().Throw<DomainException>()
            .WithMessage("ComplementNormalized deve ser vazio quando Complement não é informado.");
    }

    [Fact]
    public void Create_SemComplemento_DevePersistirComplementNormalizedVazio()
    {
        var property = Property.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua A",
            "rua a",
            "100");

        property.Complement.Should().BeNull();
        property.ComplementNormalized.Should().BeEmpty();
    }

    [Fact]
    public void UpdateAddress_ComDadosValidos_DeveAtualizarEnderecoIsActiveEUpdatedAt()
    {
        var property = CreateValidProperty("property-1");

        property.UpdateAddress(
            "city-2",
            "Contagem",
            "Savassi",
            "savassi",
            "Rua Pernambuco",
            "rua pernambuco",
            "200",
            null,
            string.Empty,
            "IDX-2",
            isActive: false);

        property.CityId.Should().Be("city-2");
        property.CityNameSnapshot.Should().Be("Contagem");
        property.Number.Should().Be("200");
        property.IsActive.Should().BeFalse();
        property.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void GenerateAddressKey_DeveMontarChaveComComplementNormalized()
    {
        var property = CreateValidProperty("property-1");

        property.GenerateAddressKey().Should().Be("city-1|centro|rua a|100|APT 101");
    }

    [Fact]
    public void GenerateAddressKey_ComComplementosEquivalentesNormalizados_DeveSerIdentica()
    {
        var propertyA = Property.Create(
            "property-a",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua A",
            "rua a",
            "100",
            "Apartamento 303 Bloco A",
            "APT 303 BLOCO A");

        var propertyB = Property.Create(
            "property-b",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua A",
            "rua a",
            "100",
            "APT 303 BLOCO A",
            "APT 303 BLOCO A");

        propertyA.GenerateAddressKey().Should().Be(propertyB.GenerateAddressKey());
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
            "Apartamento 101",
            "APT 101");
}
