using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Application.Services;
using AcervoImobiliario.Application.UnitTests.Common;
using FluentAssertions;
using NSubstitute;

namespace AcervoImobiliario.Application.UnitTests.Services;

public class PropertyAutocompleteServiceTests
{
    private readonly IPropertyRepository _propertyRepository = Substitute.For<IPropertyRepository>();
    private readonly ITextNormalizer _textNormalizer = Substitute.For<ITextNormalizer>();
    private readonly PropertyAutocompleteService _sut;

    public PropertyAutocompleteServiceTests()
    {
        _sut = new PropertyAutocompleteService(_propertyRepository, _textNormalizer);
    }

    [Fact]
    public async Task SearchNeighborhoodsAsync_ComQueryValida_DeveNormalizarTermoEBuscarNoRepositorio()
    {
        // Arrange
        var query = new SearchNeighborhoodsAutocompleteQuery("city-1", " Ce ");
        _textNormalizer.Normalize(" Ce ").Returns("ce");
        _propertyRepository.SearchDistinctNeighborhoodsAsync("city-1", "ce", Arg.Any<CancellationToken>())
            .Returns(new List<string> { "Centro", "Cidade Nova" });

        // Act
        var result = await _sut.SearchNeighborhoodsAsync(query);

        // Assert
        result.ShouldBeSuccess().Should().BeEquivalentTo(new[] { "Centro", "Cidade Nova" });
        await _propertyRepository.Received(1).SearchDistinctNeighborhoodsAsync(
            "city-1",
            "ce",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchStreetsAsync_ComQueryValida_DeveNormalizarNeighborhoodETermo()
    {
        // Arrange
        var query = new SearchStreetsAutocompleteQuery("city-1", " Centro ", " Ru ");
        _textNormalizer.Normalize(" Centro ").Returns("centro");
        _textNormalizer.Normalize(" Ru ").Returns("ru");
        _propertyRepository.SearchDistinctStreetsAsync("city-1", "centro", "ru", Arg.Any<CancellationToken>())
            .Returns(new List<string> { "Rua A" });

        // Act
        var result = await _sut.SearchStreetsAsync(query);

        // Assert
        result.ShouldBeSuccess().Should().ContainSingle().Which.Should().Be("Rua A");
    }

    [Fact]
    public async Task SearchNumbersAsync_ComQueryValida_DeveNormalizarEnderecoEBuscarNumeros()
    {
        // Arrange
        var query = new SearchNumbersAutocompleteQuery("city-1", "Centro", "Rua A", "1");
        _textNormalizer.Normalize("Centro").Returns("centro");
        _textNormalizer.Normalize("Rua A").Returns("rua a");
        _propertyRepository.SearchDistinctNumbersAsync("city-1", "centro", "rua a", "1", Arg.Any<CancellationToken>())
            .Returns(new List<string> { "1", "10", "100" });

        // Act
        var result = await _sut.SearchNumbersAsync(query);

        // Assert
        result.ShouldBeSuccess().Should().BeEquivalentTo(new[] { "1", "10", "100" });
    }

    [Fact]
    public async Task SearchNeighborhoodsAsync_ComTermInvalido_DeveLancarValidationException()
    {
        // Arrange
        var query = new SearchNeighborhoodsAutocompleteQuery("city-1", "a");

        // Act
        var result = await _sut.SearchNeighborhoodsAsync(query);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        await _propertyRepository.DidNotReceive().SearchDistinctNeighborhoodsAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }
}
