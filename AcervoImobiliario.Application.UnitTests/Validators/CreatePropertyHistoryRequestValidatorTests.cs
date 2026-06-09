using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.PropertyHistories;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Application.Validators;
using AcervoImobiliario.Domain.Enums;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Validators;

public class CreatePropertyHistoryRequestValidatorTests
{
    private static CreatePropertyHistoryRequest ValidRequest() =>
        new(PropertyHistoryEventType.Sale, DateTime.UtcNow, "Venda registrada");

    [Fact]
    public void Validate_ComRequestValido_NaoDeveLancarExcecao()
    {
        // Arrange
        var request = ValidRequest();

        // Act
        var result = CreatePropertyHistoryRequestValidator.Validate(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ComDescricaoInvalida_DeveLancarValidationException(string description)
    {
        // Arrange
        var request = ValidRequest() with { Description = description };

        // Act
        var result = CreatePropertyHistoryRequestValidator.Validate(request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain("A descrição do evento é obrigatória.");
    }

    [Fact]
    public void Validate_ComEventDatePadrao_DeveLancarValidationException()
    {
        // Arrange
        var request = ValidRequest() with { EventDate = default };

        // Act
        var result = CreatePropertyHistoryRequestValidator.Validate(request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain("A data do evento é obrigatória.");
    }
}
