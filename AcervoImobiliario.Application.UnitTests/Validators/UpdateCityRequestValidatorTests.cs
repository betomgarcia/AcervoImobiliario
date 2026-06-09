using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Cities;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Application.Validators;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Validators;

public class UpdateCityRequestValidatorTests
{
    [Fact]
    public void Validate_ComRequestValido_NaoDeveLancarExcecao()
    {
        // Arrange
        var request = new UpdateCityRequest("Contagem", "MG", true);

        // Act
        var result = UpdateCityRequestValidator.Validate(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComEstadoVazio_DeveLancarValidationException()
    {
        // Arrange
        var request = new UpdateCityRequest("Contagem", "", true);

        // Act
        var result = UpdateCityRequestValidator.Validate(request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain("O estado da cidade é obrigatório.");
    }
}
