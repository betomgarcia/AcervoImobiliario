using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.Validators;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Validators;

public class UpdatePropertyRequestValidatorTests
{
    private static UpdatePropertyRequest ValidRequest() =>
        new("city-1", "Centro", "Rua das Flores", "100", null, null, true);

    [Fact]
    public void Validate_ComRequestValido_DeveRetornarSucesso()
    {
        var result = UpdatePropertyRequestValidator.Validate(ValidRequest());

        result.IsSuccess.Should().BeTrue();
    }
}
