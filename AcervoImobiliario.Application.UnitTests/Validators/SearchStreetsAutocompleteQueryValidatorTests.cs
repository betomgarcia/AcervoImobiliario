using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Application.Validators;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Validators;

public class SearchStreetsAutocompleteQueryValidatorTests
{
    [Fact]
    public void Validate_ComQueryValida_NaoDeveLancarExcecao()
    {
        // Arrange
        var query = new SearchStreetsAutocompleteQuery("city-1", "Centro", "ru");

        // Act
        var result = SearchStreetsAutocompleteQueryValidator.Validate(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_SemNeighborhood_DeveLancarValidationException()
    {
        // Arrange
        var query = new SearchStreetsAutocompleteQuery("city-1", "", "rua");

        // Act
        var result = SearchStreetsAutocompleteQueryValidator.Validate(query);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain("O parâmetro neighborhood é obrigatório.");
    }
}
