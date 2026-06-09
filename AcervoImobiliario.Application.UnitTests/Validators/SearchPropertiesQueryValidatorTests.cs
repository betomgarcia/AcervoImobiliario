using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Application.Validators;
using AcervoImobiliario.Domain.Enums;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Validators;

public class SearchPropertiesQueryValidatorTests
{
    [Theory]
    [InlineData("city-1", null, null, null, null, null, null)]
    [InlineData("city-1", "Centro", null, null, null, null, null)]
    [InlineData("city-1", "Centro", "Rua A", null, null, null, null)]
    [InlineData("city-1", "Centro", "Rua A", "100", null, null, null)]
    [InlineData("city-1", "Centro", "Rua A", "100", ComplementType.Apartment, "Apto 1", null)]
    [InlineData(null, null, null, null, null, null, "IDX-1")]
    public void Validate_ComCombinacoesPermitidas_NaoDeveLancarExcecao(
        string? cityId,
        string? neighborhood,
        string? street,
        string? number,
        ComplementType? complementType,
        string? complementValue,
        string? cadastralIndex)
    {
        // Arrange
        var query = new SearchPropertiesQuery(
            cityId,
            neighborhood,
            street,
            number,
            complementType,
            complementValue,
            cadastralIndex);

        // Act
        var result = SearchPropertiesQueryValidator.Validate(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_SemFiltros_DeveLancarValidationException()
    {
        // Arrange
        var query = new SearchPropertiesQuery();

        // Act
        var result = SearchPropertiesQueryValidator.Validate(query);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation, "Informe cityId ou cadastralIndex para realizar a busca.");
    }

    [Theory]
    [InlineData(null, null, null, "100", null, null, null, "Não é permitido buscar somente por number.")]
    [InlineData(null, null, null, null, ComplementType.Apartment, null, null, "Não é permitido buscar somente por complementType.")]
    [InlineData(null, null, null, null, null, "Apto 1", null, "Não é permitido buscar somente por complementValue.")]
    [InlineData(null, "Centro", null, null, null, null, null, "Não é permitido buscar por neighborhood sem cityId.")]
    [InlineData("city-1", null, "Rua A", null, null, null, null, "Não é permitido buscar por street sem cityId e neighborhood.")]
    [InlineData("city-1", null, null, "100", null, null, null, "Não é permitido buscar por number sem cityId, neighborhood e street.")]
    [InlineData("city-1", "Centro", null, "100", null, null, null, "Não é permitido buscar por number sem cityId, neighborhood e street.")]
    public void Validate_ComCombinacoesInvalidas_DeveLancarValidationException(
        string? cityId,
        string? neighborhood,
        string? street,
        string? number,
        ComplementType? complementType,
        string? complementValue,
        string? cadastralIndex,
        string expectedMessage)
    {
        // Arrange
        var query = new SearchPropertiesQuery(
            cityId,
            neighborhood,
            street,
            number,
            complementType,
            complementValue,
            cadastralIndex);

        // Act
        var result = SearchPropertiesQueryValidator.Validate(query);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation, expectedMessage);
    }

    [Fact]
    public void Validate_ComCadastralIndexEOutrosFiltros_DeveLancarValidationException()
    {
        // Arrange
        var query = new SearchPropertiesQuery(CityId: "city-1", CadastralIndex: "IDX-1");

        // Act
        var result = SearchPropertiesQueryValidator.Validate(query);

        // Assert
        result.ShouldBeFailure(
            ErrorKind.Validation,
            "A busca por cadastralIndex não pode ser combinada com outros filtros de endereço.");
    }

    [Fact]
    public void Validate_ComComplementTypeSemComplementValue_DeveLancarValidationException()
    {
        // Arrange
        var query = new SearchPropertiesQuery(
            "city-1",
            "Centro",
            "Rua A",
            "100",
            ComplementType.Apartment);

        // Act
        var result = SearchPropertiesQueryValidator.Validate(query);

        // Assert
        result.ShouldBeFailure(
            ErrorKind.Validation,
            "complementValue é obrigatório quando complementType for informado na busca.");
    }
}
