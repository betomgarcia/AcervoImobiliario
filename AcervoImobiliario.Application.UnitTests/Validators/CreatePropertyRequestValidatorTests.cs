using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Application.Validators;
using AcervoImobiliario.Domain.Enums;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Validators;

public class CreatePropertyRequestValidatorTests
{
    private static CreatePropertyRequest ValidRequest() =>
        new("city-1", "Centro", "Rua das Flores", "100", ComplementType.None, null, null);

    [Fact]
    public void Validate_ComRequestValido_NaoDeveLancarExcecao()
    {
        // Arrange
        var request = ValidRequest();

        // Act
        var result = CreatePropertyRequestValidator.Validate(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ComCityIdInvalido_DeveLancarValidationException(string cityId)
    {
        // Arrange
        var request = ValidRequest() with { CityId = cityId };

        // Act
        var result = CreatePropertyRequestValidator.Validate(request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain("O identificador da cidade é obrigatório.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ComBairroInvalido_DeveLancarValidationException(string neighborhood)
    {
        // Arrange
        var request = ValidRequest() with { Neighborhood = neighborhood };

        // Act
        var result = CreatePropertyRequestValidator.Validate(request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain("O bairro é obrigatório.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ComRuaInvalida_DeveLancarValidationException(string street)
    {
        // Arrange
        var request = ValidRequest() with { Street = street };

        // Act
        var result = CreatePropertyRequestValidator.Validate(request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain("A rua é obrigatória.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("10A")]
    [InlineData("S/N")]
    public void Validate_ComNumeroInvalido_DeveLancarValidationException(string number)
    {
        // Arrange
        var request = ValidRequest() with { Number = number };

        // Act
        var result = CreatePropertyRequestValidator.Validate(request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().ContainSingle(error =>
            error == "O número é obrigatório." || error == "O número deve conter somente dígitos.");
    }

    [Theory]
    [InlineData(ComplementType.Apartment)]
    [InlineData(ComplementType.Room)]
    [InlineData(ComplementType.Store)]
    public void Validate_ComComplementoObrigatorioSemValor_DeveLancarValidationException(ComplementType complementType)
    {
        // Arrange
        var request = ValidRequest() with { ComplementType = complementType, ComplementValue = null };

        // Act
        var result = CreatePropertyRequestValidator.Validate(request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().ContainSingle(error =>
            error == $"ComplementValue é obrigatório quando ComplementType for {complementType}.");
    }

    [Fact]
    public void Validate_ComComplementTypeNoneEValorInformado_DeveLancarValidationException()
    {
        // Arrange
        var request = ValidRequest() with { ComplementType = ComplementType.None, ComplementValue = "Apto 1" };

        // Act
        var result = CreatePropertyRequestValidator.Validate(request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain("ComplementValue deve ser vazio ou nulo quando ComplementType for None.");
    }
}
