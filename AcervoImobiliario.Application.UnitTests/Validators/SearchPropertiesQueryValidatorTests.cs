using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.Validators;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Validators;

public class SearchPropertiesQueryValidatorTests
{
    [Theory]
    [InlineData("city-1", "Centro", "Rua A", "100", "Apto 303 Bloco A", null)]
    public void Validate_ComEnderecoCompleto_DeveRetornarSucesso(
        string? cityId,
        string? neighborhood,
        string? street,
        string? number,
        string? complement,
        string? cadastralIndex)
    {
        var query = new SearchPropertiesQuery(
            cityId,
            neighborhood,
            street,
            number,
            complement,
            cadastralIndex);

        var result = SearchPropertiesQueryValidator.Validate(query);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(null, null, null, "100", null, null, "Não é permitido buscar somente por number.")]
    [InlineData(null, null, null, null, "Apto 1", null, "Não é permitido buscar somente por complement.")]
    [InlineData(null, "Centro", null, null, null, null, "Não é permitido buscar por neighborhood sem cityId.")]
    [InlineData("city-1", null, "Rua A", null, null, null, "Não é permitido buscar por street sem cityId e neighborhood.")]
    [InlineData("city-1", "Centro", null, "100", null, null, "Não é permitido buscar por number sem cityId, neighborhood e street.")]
    [InlineData("city-1", "Centro", "Rua A", null, "Apto 1", null, "complement exige cityId, neighborhood, street e number.")]
    public void Validate_ComCombinacaoInvalida_DeveRetornarErroDeValidacao(
        string? cityId,
        string? neighborhood,
        string? street,
        string? number,
        string? complement,
        string? cadastralIndex,
        string expectedMessage)
    {
        var query = new SearchPropertiesQuery(
            cityId,
            neighborhood,
            street,
            number,
            complement,
            cadastralIndex);

        var result = SearchPropertiesQueryValidator.Validate(query);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public void Validate_ComCadastralIndexCombinadoComEndereco_DeveRetornarErroDeValidacao()
    {
        var query = new SearchPropertiesQuery(
            CityId: "city-1",
            CadastralIndex: "IDX-1");

        var result = SearchPropertiesQueryValidator.Validate(query);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Message.Should().Be(
            "A busca por cadastralIndex não pode ser combinada com outros filtros de endereço.");
    }
}
