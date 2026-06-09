using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Application.Validators;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Validators;

public class SearchNumbersAutocompleteQueryValidatorTests
{
    [Fact]
    public void Validate_ComQueryValida_NaoDeveLancarExcecao()
    {
        // Arrange
        var query = new SearchNumbersAutocompleteQuery("city-1", "Centro", "Rua A", "1");

        // Act
        var result = SearchNumbersAutocompleteQueryValidator.Validate(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_SemStreet_DeveLancarValidationException()
    {
        // Arrange
        var query = new SearchNumbersAutocompleteQuery("city-1", "Centro", "", "1");

        // Act
        var result = SearchNumbersAutocompleteQueryValidator.Validate(query);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain("O parâmetro street é obrigatório.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ComTermVazio_DeveLancarValidationException(string term)
    {
        // Arrange
        var query = new SearchNumbersAutocompleteQuery("city-1", "Centro", "Rua A", term);

        // Act
        var result = SearchNumbersAutocompleteQueryValidator.Validate(query);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain("O parâmetro term deve conter no mínimo 1 caractere.");
    }
}
