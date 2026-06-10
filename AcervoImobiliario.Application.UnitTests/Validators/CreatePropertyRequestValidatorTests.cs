using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.Validators;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Validators;

public class CreatePropertyRequestValidatorTests
{
    private static CreatePropertyRequest ValidRequest() =>
        new("city-1", "Centro", "Rua das Flores", "100", null, null);

    [Fact]
    public void Validate_ComRequestValido_DeveRetornarSucesso()
    {
        var result = CreatePropertyRequestValidator.Validate(ValidRequest());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Validate_ComComplementoInformado_DeveRetornarSucesso()
    {
        var request = ValidRequest() with { Complement = "Apartamento 303 Bloco A" };

        var result = CreatePropertyRequestValidator.Validate(request);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Centro", "Rua A", "100")]
    [InlineData("city-1", "", "Rua A", "100")]
    [InlineData("city-1", "Centro", "", "100")]
    [InlineData("city-1", "Centro", "Rua A", "")]
    public void Validate_ComCamposObrigatoriosVazios_DeveRetornarErroDeValidacao(
        string cityId,
        string neighborhood,
        string street,
        string number)
    {
        var request = new CreatePropertyRequest(cityId, neighborhood, street, number, null, null);

        var result = CreatePropertyRequestValidator.Validate(request);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Kind.Should().Be(ErrorKind.Validation);
    }
}
