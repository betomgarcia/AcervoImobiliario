using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Cities;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Application.Validators;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Validators;

public class CreateCityRequestValidatorTests
{
    [Fact]
    public void Validate_ComRequestValido_NaoDeveLancarExcecao()
    {
        // Arrange
        var request = new CreateCityRequest("Belo Horizonte", "MG");

        // Act
        var result = CreateCityRequestValidator.Validate(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComNomeVazio_DeveLancarValidationException()
    {
        // Arrange
        var request = new CreateCityRequest("", "MG");

        // Act
        var result = CreateCityRequestValidator.Validate(request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain("O nome da cidade é obrigatório.");
    }

    [Fact]
    public void Validate_ComEstadoInvalido_DeveLancarValidationException()
    {
        // Arrange
        var request = new CreateCityRequest("Belo Horizonte", "Minas");

        // Act
        var result = CreateCityRequestValidator.Validate(request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain("O estado deve conter exatamente 2 caracteres.");
    }
}
