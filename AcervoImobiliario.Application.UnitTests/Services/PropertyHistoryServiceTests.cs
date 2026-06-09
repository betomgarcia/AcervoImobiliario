using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.PropertyHistories;
using AcervoImobiliario.Application.Enums;
using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Application.Services;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Domain.Entities;
using AcervoImobiliario.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace AcervoImobiliario.Application.UnitTests.Services;

public class PropertyHistoryServiceTests
{
    private readonly IPropertyHistoryRepository _historyRepository = Substitute.For<IPropertyHistoryRepository>();
    private readonly IPropertyRepository _propertyRepository = Substitute.For<IPropertyRepository>();
    private readonly PropertyHistoryService _sut;

    public PropertyHistoryServiceTests()
    {
        _sut = new PropertyHistoryService(_historyRepository, _propertyRepository);
    }

    private static CreatePropertyHistoryRequest ValidRequest() =>
        new(PropertyHistoryEventType.Note, DateTime.UtcNow, "Observação registrada");

    private static Property CreateProperty(string id = "property-1") =>
        Property.Create(
            id,
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua A",
            "rua a",
            "100",
            ComplementType.None);

    [Fact]
    public async Task CreateAsync_ComDadosValidos_DeveRegistrarHistorico()
    {
        // Arrange
        const string propertyId = "property-1";
        var request = ValidRequest();
        _propertyRepository.GetByIdAsync(propertyId, Arg.Any<CancellationToken>())
            .Returns(CreateProperty(propertyId));

        // Act
        var result = await _sut.CreateAsync(propertyId, request);

        // Assert
        var response = result.ShouldBeSuccess();
        response.PropertyId.Should().Be(propertyId);
        response.EventType.Should().Be(request.EventType);
        response.Description.Should().Be(request.Description);
        response.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        await _historyRepository.Received(1).CreateAsync(
            Arg.Is<PropertyHistory>(history => history.PropertyId == propertyId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ComImovelInexistente_DeveLancarNotFoundException()
    {
        // Arrange
        _propertyRepository.GetByIdAsync("inexistente", Arg.Any<CancellationToken>())
            .Returns((Property?)null);

        // Act
        var result = await _sut.CreateAsync("inexistente", ValidRequest());

        // Assert
        result.ShouldBeFailure(ErrorKind.NotFound, "Imóvel 'inexistente' não encontrado.");

        await _historyRepository.DidNotReceive().CreateAsync(
            Arg.Any<PropertyHistory>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ComRequestInvalido_DeveLancarValidationException()
    {
        // Arrange
        var request = ValidRequest() with { Description = "" };

        // Act
        var result = await _sut.CreateAsync("property-1", request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        await _propertyRepository.DidNotReceive().GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ListByPropertyIdAsync_ComSortDirectionDesc_DeveListarHistorico()
    {
        // Arrange
        const string propertyId = "property-1";
        var history = PropertyHistory.Create(
            "history-1",
            propertyId,
            PropertyHistoryEventType.Sale,
            DateTime.UtcNow,
            "Venda");

        _propertyRepository.GetByIdAsync(propertyId, Arg.Any<CancellationToken>())
            .Returns(CreateProperty(propertyId));
        _historyRepository.ListByPropertyIdAsync(
            propertyId,
            HistorySortDirection.Desc,
            Arg.Any<CancellationToken>()).Returns(new List<PropertyHistory> { history });

        // Act
        var result = await _sut.ListByPropertyIdAsync(propertyId);

        // Assert
        var histories = result.ShouldBeSuccess();
        histories.Should().ContainSingle();
        histories[0].Id.Should().Be("history-1");
    }

    [Fact]
    public async Task ListByPropertyIdAsync_ComSortDirectionAsc_DeveRepasseOrdenacao()
    {
        // Arrange
        const string propertyId = "property-1";
        _propertyRepository.GetByIdAsync(propertyId, Arg.Any<CancellationToken>())
            .Returns(CreateProperty(propertyId));
        _historyRepository.ListByPropertyIdAsync(
            propertyId,
            HistorySortDirection.Asc,
            Arg.Any<CancellationToken>()).Returns(new List<PropertyHistory>());

        // Act
        await _sut.ListByPropertyIdAsync(propertyId, "asc");

        // Assert
        await _historyRepository.Received(1).ListByPropertyIdAsync(
            propertyId,
            HistorySortDirection.Asc,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ListByPropertyIdAsync_ComImovelInexistente_DeveLancarNotFoundException()
    {
        // Arrange
        _propertyRepository.GetByIdAsync("inexistente", Arg.Any<CancellationToken>())
            .Returns((Property?)null);

        // Act
        var result = await _sut.ListByPropertyIdAsync("inexistente");

        // Assert
        result.ShouldBeFailure(ErrorKind.NotFound, "Imóvel 'inexistente' não encontrado.");
    }
}
