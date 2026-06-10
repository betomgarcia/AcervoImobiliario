using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.Validators;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Validators;

public class PropertyAddressValidatorTests
{
    [Theory]
    [InlineData("10A")]
    [InlineData("S/N")]
    public void Validate_ComNumeroInvalido_DeveRetornarErroDeValidacao(string number)
    {
        var result = PropertyAddressValidator.Validate(
            "city-1", "Centro", "Rua A", number);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Errors.Should().Contain("O número deve conter somente dígitos.");
    }

    [Fact]
    public void Validate_ComEnderecoValido_DeveRetornarSucesso()
    {
        var result = PropertyAddressValidator.Validate(
            "city-1", "Centro", "Rua A", "100");

        result.IsSuccess.Should().BeTrue();
    }
}
