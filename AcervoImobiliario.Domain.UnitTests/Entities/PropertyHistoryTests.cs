using AcervoImobiliario.Domain.Entities;
using AcervoImobiliario.Domain.Enums;
using AcervoImobiliario.Domain.Exceptions;
using FluentAssertions;

namespace AcervoImobiliario.Domain.UnitTests.Entities;

public class PropertyHistoryTests
{
    [Fact]
    public void Create_ComDadosValidos_DeveCriarHistorico()
    {
        // Arrange
        const string id = "history-1";
        const string propertyId = "property-1";
        var eventDate = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Local);

        // Act
        var history = PropertyHistory.Create(
            id,
            propertyId,
            PropertyHistoryEventType.Sale,
            eventDate,
            "Venda registrada");

        // Assert
        history.Id.Should().Be(id);
        history.PropertyId.Should().Be(propertyId);
        history.EventType.Should().Be(PropertyHistoryEventType.Sale);
        history.EventDate.Should().Be(eventDate.ToUniversalTime());
        history.Description.Should().Be("Venda registrada");
        history.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ComDescricaoInvalida_DeveLancarDomainException(string description)
    {
        // Arrange & Act
        var act = () => PropertyHistory.Create(
            "history-1",
            "property-1",
            PropertyHistoryEventType.Note,
            DateTime.UtcNow,
            description);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("A descrição do evento é obrigatória.");
    }
}
