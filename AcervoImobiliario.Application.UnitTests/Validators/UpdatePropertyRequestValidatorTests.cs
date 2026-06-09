using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Application.Validators;
using AcervoImobiliario.Domain.Enums;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Validators;

public class UpdatePropertyRequestValidatorTests
{
    private static UpdatePropertyRequest ValidRequest() =>
        new("city-1", "Centro", "Rua das Flores", "100", ComplementType.None, null, null, true);

    [Fact]
    public void Validate_ComRequestValido_NaoDeveLancarExcecao()
    {
        // Arrange
        var request = ValidRequest();

        // Act
        var result = UpdatePropertyRequestValidator.Validate(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComNumeroInvalido_DeveLancarValidationException()
    {
        // Arrange
        var request = ValidRequest() with { Number = "10A" };

        // Act
        var result = UpdatePropertyRequestValidator.Validate(request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain("O número deve conter somente dígitos.");
    }
}
