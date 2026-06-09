using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Cities;
using AcervoImobiliario.Application.Factories;
using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Application.Services;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace AcervoImobiliario.Application.UnitTests.Services;

public class CityServiceTests
{
    private readonly ICityRepository _cityRepository = Substitute.For<ICityRepository>();
    private readonly ICityFactory _cityFactory = Substitute.For<ICityFactory>();
    private readonly ITextNormalizer _textNormalizer = Substitute.For<ITextNormalizer>();
    private readonly CityService _sut;

    public CityServiceTests()
    {
        _sut = new CityService(_cityRepository, _cityFactory, _textNormalizer);
    }

    [Fact]
    public async Task ListActiveAsync_DeveRetornarSomenteCidadesAtivasMapeadas()
    {
        // Arrange
        var activeCity = City.Restore(
            "city-1",
            "Belo Horizonte",
            "belo horizonte",
            "MG",
            isActive: true,
            DateTime.UtcNow,
            null);

        _cityRepository.ListActiveAsync(Arg.Any<CancellationToken>())
            .Returns(new List<City> { activeCity });

        // Act
        var result = await _sut.ListActiveAsync();

        // Assert
        var cities = result.ShouldBeSuccess();
        cities.Should().ContainSingle();
        cities[0].Id.Should().Be("city-1");
        cities[0].Name.Should().Be("Belo Horizonte");
        cities[0].IsActive.Should().BeTrue();
        await _cityRepository.Received(1).ListActiveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchAsync_ComTermValido_DeveBuscarPorNomeNormalizado()
    {
        // Arrange
        const string term = "belo";
        const string termNormalized = "belo";

        var city = City.Restore(
            "city-1",
            "Belo Horizonte",
            "belo horizonte",
            "MG",
            true,
            DateTime.UtcNow,
            null);

        _textNormalizer.Normalize(term).Returns(termNormalized);
        _cityRepository.SearchByNameNormalizedAsync(termNormalized, Arg.Any<CancellationToken>())
            .Returns(new List<City> { city });

        // Act
        var result = await _sut.SearchAsync(term);

        // Assert
        var cities = result.ShouldBeSuccess();
        cities.Should().ContainSingle();
        cities[0].NameNormalized.Should().Be("belo horizonte");
        _textNormalizer.Received(1).Normalize(term);
        await _cityRepository.Received(1).SearchByNameNormalizedAsync(termNormalized, Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("a")]
    public async Task SearchAsync_ComTermInvalido_DeveLancarValidationException(string? term)
    {
        // Act
        var result = await _sut.SearchAsync(term!);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation, "O parâmetro term deve conter no mínimo 2 caracteres.");

        await _cityRepository.DidNotReceive().SearchByNameNormalizedAsync(
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ComDadosValidos_DeveCriarCidade()
    {
        // Arrange
        var request = new CreateCityRequest("Nova Lima", "MG");
        var createdCity = City.Create("city-1", "Nova Lima", "nova lima", "MG");

        _textNormalizer.Normalize(request.Name).Returns("nova lima");
        _cityRepository.GetByNameNormalizedAndStateAsync("nova lima", "MG", Arg.Any<CancellationToken>())
            .Returns((City?)null);
        _cityFactory.Create(Arg.Any<string>(), request.Name, "MG").Returns(createdCity);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        var response = result.ShouldBeSuccess();
        response.Name.Should().Be("Nova Lima");
        response.State.Should().Be("MG");
        await _cityRepository.Received(1).CreateAsync(createdCity, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ComCidadeDuplicada_DeveLancarConflictException()
    {
        // Arrange
        var request = new CreateCityRequest("Belo Horizonte", "MG");
        var existingCity = City.Create("city-1", "Belo Horizonte", "belo horizonte", "MG");

        _textNormalizer.Normalize(request.Name).Returns("belo horizonte");
        _cityRepository.GetByNameNormalizedAndStateAsync("belo horizonte", "MG", Arg.Any<CancellationToken>())
            .Returns(existingCity);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.ShouldBeFailure(
            ErrorKind.Conflict,
            "Já existe uma cidade cadastrada com o nome 'Belo Horizonte' no estado 'MG'.");

        await _cityRepository.DidNotReceive().CreateAsync(Arg.Any<City>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ComRequestInvalido_DeveLancarValidationException()
    {
        // Arrange
        var request = new CreateCityRequest("", "MG");

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        await _cityRepository.DidNotReceive().CreateAsync(Arg.Any<City>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_ComDadosValidos_DeveAtualizarCidade()
    {
        // Arrange
        const string id = "city-1";
        var request = new UpdateCityRequest("Betim", "MG", IsActive: false);
        var city = City.Create(id, "Contagem", "contagem", "MG");

        _textNormalizer.Normalize(request.Name).Returns("betim");
        _cityRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(city);
        _cityRepository.GetByNameNormalizedAndStateAsync("betim", "MG", Arg.Any<CancellationToken>())
            .Returns((City?)null);

        // Act
        var result = await _sut.UpdateAsync(id, request);

        // Assert
        _cityFactory.Received(1).Update(city, request.Name, "MG", false);
        await _cityRepository.Received(1).UpdateAsync(city, Arg.Any<CancellationToken>());
        result.ShouldBeSuccess().Id.Should().Be(id);
    }

    [Fact]
    public async Task UpdateAsync_ComCidadeInexistente_DeveLancarNotFoundException()
    {
        // Arrange
        var request = new UpdateCityRequest("Betim", "MG", true);
        _cityRepository.GetByIdAsync("inexistente", Arg.Any<CancellationToken>())
            .Returns((City?)null);

        // Act
        var result = await _sut.UpdateAsync("inexistente", request);

        // Assert
        result.ShouldBeFailure(ErrorKind.NotFound, "Cidade 'inexistente' não encontrada.");

        await _cityRepository.DidNotReceive().UpdateAsync(Arg.Any<City>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_ComNomeDuplicadoEmOutraCidade_DeveLancarConflictException()
    {
        // Arrange
        const string id = "city-1";
        var request = new UpdateCityRequest("Betim", "MG", true);
        var city = City.Create(id, "Contagem", "contagem", "MG");
        var otherCity = City.Create("city-2", "Betim", "betim", "MG");

        _textNormalizer.Normalize(request.Name).Returns("betim");
        _cityRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(city);
        _cityRepository.GetByNameNormalizedAndStateAsync("betim", "MG", Arg.Any<CancellationToken>())
            .Returns(otherCity);

        // Act
        var result = await _sut.UpdateAsync(id, request);

        // Assert
        result.ShouldBeFailure(
            ErrorKind.Conflict,
            "Já existe outra cidade cadastrada com o nome 'Betim' no estado 'MG'.");

        await _cityRepository.DidNotReceive().UpdateAsync(Arg.Any<City>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_ComMesmaChaveDaPropriaCidade_DevePermitirAtualizacao()
    {
        // Arrange
        const string id = "city-1";
        var request = new UpdateCityRequest("Contagem", "MG", false);
        var city = City.Create(id, "Contagem", "contagem", "MG");

        _textNormalizer.Normalize(request.Name).Returns("contagem");
        _cityRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(city);
        _cityRepository.GetByNameNormalizedAndStateAsync("contagem", "MG", Arg.Any<CancellationToken>())
            .Returns(city);

        // Act
        var result = await _sut.UpdateAsync(id, request);

        // Assert
        _cityFactory.Received(1).Update(city, request.Name, "MG", false);
        await _cityRepository.Received(1).UpdateAsync(city, Arg.Any<CancellationToken>());
        result.ShouldBeSuccess().Id.Should().Be(id);
    }
}
