using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Application.Validators;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Validators;

public class SearchCitiesQueryValidatorTests
{
    [Theory]
    [InlineData("be")]
    [InlineData("belo")]
    public void Validate_ComTermoValido_NaoDeveLancarExcecao(string term)
    {
        // Act
        var result = SearchCitiesQueryValidator.Validate(term);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("b")]
    public void Validate_ComTermoInvalido_DeveLancarValidationException(string? term)
    {
        // Act
        var result = SearchCitiesQueryValidator.Validate(term!);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation, "O parâmetro term deve conter no mínimo 2 caracteres.");
    }
}
