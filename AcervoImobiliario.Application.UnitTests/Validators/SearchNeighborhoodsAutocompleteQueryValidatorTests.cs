using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Application.Validators;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Validators;

public class SearchNeighborhoodsAutocompleteQueryValidatorTests
{
    [Fact]
    public void Validate_ComQueryValida_NaoDeveLancarExcecao()
    {
        // Arrange
        var query = new SearchNeighborhoodsAutocompleteQuery("city-1", "ce");

        // Act
        var result = SearchNeighborhoodsAutocompleteQueryValidator.Validate(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_SemCityId_DeveLancarValidationException()
    {
        // Arrange
        var query = new SearchNeighborhoodsAutocompleteQuery("", "centro");

        // Act
        var result = SearchNeighborhoodsAutocompleteQueryValidator.Validate(query);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain("O parâmetro cityId é obrigatório.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("a")]
    public void Validate_ComTermInvalido_DeveLancarValidationException(string term)
    {
        // Arrange
        var query = new SearchNeighborhoodsAutocompleteQuery("city-1", term);

        // Act
        var result = SearchNeighborhoodsAutocompleteQueryValidator.Validate(query);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain("O parâmetro term deve conter no mínimo 2 caracteres.");
    }
}
