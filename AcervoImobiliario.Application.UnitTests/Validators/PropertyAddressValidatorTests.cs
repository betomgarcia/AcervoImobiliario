using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Application.Validators;
using AcervoImobiliario.Domain.Enums;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Validators;

public class PropertyAddressValidatorTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("10A")]
    [InlineData("S/N")]
    public void Validate_ComNumeroInvalido_DeveRetornarErroDeValidacao(string number)
    {
        // Act
        var result = PropertyAddressValidator.Validate(
            "city-1", "Centro", "Rua A", number, ComplementType.None, null);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain(error =>
            error == "O número é obrigatório." || error == "O número deve conter somente dígitos.");
    }

    [Theory]
    [InlineData(ComplementType.Apartment)]
    [InlineData(ComplementType.Room)]
    [InlineData(ComplementType.Store)]
    public void Validate_ComComplementoObrigatorioSemValor_DeveRetornarErroDeValidacao(ComplementType complementType)
    {
        // Act
        var result = PropertyAddressValidator.Validate(
            "city-1", "Centro", "Rua A", "100", complementType, null);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain(
            $"ComplementValue é obrigatório quando ComplementType for {complementType}.");
    }

    [Fact]
    public void Validate_ComComplementTypeNoneEValorInformado_DeveRetornarErroDeValidacao()
    {
        // Act
        var result = PropertyAddressValidator.Validate(
            "city-1", "Centro", "Rua A", "100", ComplementType.None, "Apto 1");

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain(
            "ComplementValue deve ser vazio ou nulo quando ComplementType for None.");
    }

    [Fact]
    public void Validate_ComEnderecoValidoSemComplemento_DeveRetornarSucesso()
    {
        // Act
        var result = PropertyAddressValidator.Validate(
            "city-1", "Centro", "Rua A", "100", ComplementType.None, null);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
