using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.Factories;
using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Application.Services;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Domain.Entities;
using AcervoImobiliario.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace AcervoImobiliario.Application.UnitTests.Services;

public class PropertyServiceTests
{
    private readonly IPropertyRepository _propertyRepository = Substitute.For<IPropertyRepository>();
    private readonly ICityRepository _cityRepository = Substitute.For<ICityRepository>();
    private readonly IPropertyFactory _propertyFactory = Substitute.For<IPropertyFactory>();
    private readonly ITextNormalizer _textNormalizer = Substitute.For<ITextNormalizer>();
    private readonly PropertyService _sut;

    public PropertyServiceTests()
    {
        _sut = new PropertyService(
            _propertyRepository,
            _cityRepository,
            _propertyFactory,
            _textNormalizer);
    }

    private static CreatePropertyRequest ValidRequest() =>
        new("city-1", "Centro", "Rua das Flores", "100", ComplementType.Apartment, "Apto 12", "IDX-1");

    [Fact]
    public async Task SearchAsync_ComCityId_DeveBuscarComFiltroAtivoPorPadrao()
    {
        // Arrange
        var query = new SearchPropertiesQuery(CityId: "city-1");
        var property = Property.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua A",
            "rua a",
            "100",
            ComplementType.None);

        _propertyRepository.SearchAsync(
            Arg.Is<PropertySearchCriteria>(criteria =>
                criteria.CityId == "city-1"
                && criteria.ActiveOnly
                && criteria.NeighborhoodNormalized == null),
            Arg.Any<CancellationToken>()).Returns(new List<Property> { property });

        // Act
        var result = await _sut.SearchAsync(query);

        // Assert
        var properties = result.ShouldBeSuccess();
        properties.Should().ContainSingle();
        properties[0].Id.Should().Be("property-1");
        await _propertyRepository.Received(1).SearchAsync(
            Arg.Any<PropertySearchCriteria>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchAsync_ComEnderecoCompleto_DeveNormalizarCamposTextuais()
    {
        // Arrange
        var query = new SearchPropertiesQuery(
            "city-1",
            "  Centro ",
            "  Rua das Flores ",
            "100",
            ComplementType.Apartment,
            "  Apto 12 ");

        _textNormalizer.Normalize("  Centro ").Returns("centro");
        _textNormalizer.Normalize("  Rua das Flores ").Returns("rua das flores");
        _textNormalizer.Normalize("  Apto 12 ").Returns("apto 12");
        _propertyRepository.SearchAsync(Arg.Any<PropertySearchCriteria>(), Arg.Any<CancellationToken>())
            .Returns(new List<Property>());

        // Act
        await _sut.SearchAsync(query);

        // Assert
        await _propertyRepository.Received(1).SearchAsync(
            Arg.Is<PropertySearchCriteria>(criteria =>
                criteria.NeighborhoodNormalized == "centro"
                && criteria.StreetNormalized == "rua das flores"
                && criteria.ComplementValueNormalized == "apto 12"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchAsync_ComCadastralIndex_DeveBuscarPorIndiceCadastral()
    {
        // Arrange
        var query = new SearchPropertiesQuery(CadastralIndex: " IDX-1 ");
        _propertyRepository.SearchAsync(Arg.Any<PropertySearchCriteria>(), Arg.Any<CancellationToken>())
            .Returns(new List<Property>());

        // Act
        await _sut.SearchAsync(query);

        // Assert
        await _propertyRepository.Received(1).SearchAsync(
            Arg.Is<PropertySearchCriteria>(criteria =>
                criteria.CadastralIndex == "IDX-1"
                && criteria.CityId == null
                && criteria.ActiveOnly),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchAsync_ComIncludeInactive_DeveDesativarFiltroDeAtivos()
    {
        // Arrange
        var query = new SearchPropertiesQuery(CityId: "city-1", IncludeInactive: true);
        _propertyRepository.SearchAsync(Arg.Any<PropertySearchCriteria>(), Arg.Any<CancellationToken>())
            .Returns(new List<Property>());

        // Act
        await _sut.SearchAsync(query);

        // Assert
        await _propertyRepository.Received(1).SearchAsync(
            Arg.Is<PropertySearchCriteria>(criteria => !criteria.ActiveOnly),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchAsync_ComCombinacaoInvalida_DeveLancarValidationException()
    {
        // Arrange
        var query = new SearchPropertiesQuery(Number: "100");

        // Act
        var result = await _sut.SearchAsync(query);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        await _propertyRepository.DidNotReceive().SearchAsync(
            Arg.Any<PropertySearchCriteria>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ComDadosValidos_DeveCriarImovel()
    {
        // Arrange
        var request = ValidRequest();
        var city = City.Create("city-1", "Belo Horizonte", "belo horizonte", "MG");
        var property = Property.Create(
            "property-1",
            city.Id,
            city.Name,
            request.Neighborhood,
            "centro",
            request.Street,
            "rua das flores",
            request.Number,
            request.ComplementType,
            request.ComplementValue,
            "apto 12",
            request.CadastralIndex);

        _cityRepository.GetByIdAsync(request.CityId, Arg.Any<CancellationToken>()).Returns(city);
        _textNormalizer.Normalize(request.Neighborhood).Returns("centro");
        _textNormalizer.Normalize(request.Street).Returns("rua das flores");
        _textNormalizer.Normalize(request.ComplementValue!).Returns("apto 12");
        _propertyRepository.GetByUniqueAddressAsync(
            city.Id,
            "centro",
            "rua das flores",
            request.Number,
            request.ComplementType,
            "apto 12",
            Arg.Any<CancellationToken>()).Returns((Property?)null);
        _propertyFactory.Create(
            Arg.Any<string>(),
            city.Id,
            city.Name,
            request.Neighborhood,
            request.Street,
            request.Number,
            request.ComplementType,
            request.ComplementValue,
            request.CadastralIndex).Returns(property);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        var response = result.ShouldBeSuccess();
        response.Id.Should().Be("property-1");
        response.CityNameSnapshot.Should().Be("Belo Horizonte");
        response.IsActive.Should().BeTrue();
        response.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        await _propertyRepository.Received(1).CreateAsync(property, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ComCidadeInexistente_DeveLancarNotFoundException()
    {
        // Arrange
        var request = ValidRequest();
        _cityRepository.GetByIdAsync(request.CityId, Arg.Any<CancellationToken>())
            .Returns((City?)null);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.ShouldBeFailure(ErrorKind.NotFound, $"Cidade '{request.CityId}' não encontrada.");

        await _propertyRepository.DidNotReceive().CreateAsync(Arg.Any<Property>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ComEnderecoDuplicado_DeveLancarConflictException()
    {
        // Arrange
        var request = ValidRequest();
        var city = City.Create("city-1", "Belo Horizonte", "belo horizonte", "MG");
        var existingProperty = Property.Create(
            "property-existing",
            city.Id,
            city.Name,
            request.Neighborhood,
            "centro",
            request.Street,
            "rua das flores",
            request.Number,
            request.ComplementType,
            request.ComplementValue,
            "apto 12",
            null);

        _cityRepository.GetByIdAsync(request.CityId, Arg.Any<CancellationToken>()).Returns(city);
        _textNormalizer.Normalize(request.Neighborhood).Returns("centro");
        _textNormalizer.Normalize(request.Street).Returns("rua das flores");
        _textNormalizer.Normalize(request.ComplementValue!).Returns("apto 12");
        _propertyRepository.GetByUniqueAddressAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<ComplementType>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>()).Returns(existingProperty);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Conflict, "Já existe um imóvel cadastrado para este endereço.");

        await _propertyRepository.DidNotReceive().CreateAsync(Arg.Any<Property>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ComRequestInvalido_DeveLancarValidationException()
    {
        // Arrange
        var request = ValidRequest() with { Number = "10A" };

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation);
        await _cityRepository.DidNotReceive().GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_ComImovelExistente_DeveRetornarImovel()
    {
        // Arrange
        var property = Property.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua das Flores",
            "rua das flores",
            "100",
            ComplementType.None);

        _propertyRepository.GetByIdAsync("property-1", Arg.Any<CancellationToken>()).Returns(property);

        // Act
        var result = await _sut.GetByIdAsync("property-1");

        // Assert
        var response = result.ShouldBeSuccess();
        response.Id.Should().Be("property-1");
        response.CityNameSnapshot.Should().Be("Belo Horizonte");
    }

    [Fact]
    public async Task GetByIdAsync_ComImovelInexistente_DeveLancarNotFoundException()
    {
        // Arrange
        _propertyRepository.GetByIdAsync("inexistente", Arg.Any<CancellationToken>())
            .Returns((Property?)null);

        // Act
        var result = await _sut.GetByIdAsync("inexistente");

        // Assert
        result.ShouldBeFailure(ErrorKind.NotFound, "Imóvel 'inexistente' não encontrado.");
    }

    private static UpdatePropertyRequest ValidUpdateRequest() =>
        new("city-2", "Savassi", "Rua Pernambuco", "200", ComplementType.Apartment, "Apto 3", "IDX-2", false);

    [Fact]
    public async Task UpdateAsync_ComDadosValidos_DeveAtualizarImovel()
    {
        // Arrange
        const string propertyId = "property-1";
        var request = ValidUpdateRequest();
        var property = Property.Create(
            propertyId,
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua das Flores",
            "rua das flores",
            "100",
            ComplementType.Apartment,
            "Apto 12",
            "apto 12",
            "IDX-1");
        var newCity = City.Create("city-2", "Contagem", "contagem", "MG");

        _propertyRepository.GetByIdAsync(propertyId, Arg.Any<CancellationToken>()).Returns(property);
        _cityRepository.GetByIdAsync(request.CityId, Arg.Any<CancellationToken>()).Returns(newCity);
        _textNormalizer.Normalize(request.Neighborhood).Returns("savassi");
        _textNormalizer.Normalize(request.Street).Returns("rua pernambuco");
        _textNormalizer.Normalize(request.ComplementValue!).Returns("apto 3");
        _propertyRepository.GetByUniqueAddressAsync(
            newCity.Id,
            "savassi",
            "rua pernambuco",
            request.Number,
            request.ComplementType,
            "apto 3",
            Arg.Any<CancellationToken>()).Returns((Property?)null);

        // Act
        var result = await _sut.UpdateAsync(propertyId, request);

        // Assert
        _propertyFactory.Received(1).UpdateAddress(
            property,
            newCity.Id,
            newCity.Name,
            request.Neighborhood,
            request.Street,
            request.Number,
            request.ComplementType,
            request.ComplementValue,
            request.CadastralIndex,
            request.IsActive);
        await _propertyRepository.Received(1).UpdateAsync(property, Arg.Any<CancellationToken>());
        result.ShouldBeSuccess().Id.Should().Be(propertyId);
    }

    [Fact]
    public async Task UpdateAsync_ComImovelInexistente_DeveLancarNotFoundException()
    {
        // Arrange
        _propertyRepository.GetByIdAsync("inexistente", Arg.Any<CancellationToken>())
            .Returns((Property?)null);

        // Act
        var result = await _sut.UpdateAsync("inexistente", ValidUpdateRequest());

        // Assert
        result.ShouldBeFailure(ErrorKind.NotFound, "Imóvel 'inexistente' não encontrado.");
    }

    [Fact]
    public async Task UpdateAsync_ComCidadeInexistente_DeveLancarNotFoundException()
    {
        // Arrange
        var property = Property.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua das Flores",
            "rua das flores",
            "100",
            ComplementType.None);

        _propertyRepository.GetByIdAsync("property-1", Arg.Any<CancellationToken>()).Returns(property);
        _cityRepository.GetByIdAsync("city-inexistente", Arg.Any<CancellationToken>())
            .Returns((City?)null);

        var request = ValidUpdateRequest() with { CityId = "city-inexistente" };

        // Act
        var result = await _sut.UpdateAsync("property-1", request);

        // Assert
        result.ShouldBeFailure(ErrorKind.NotFound, "Cidade 'city-inexistente' não encontrada.");
    }

    [Fact]
    public async Task UpdateAsync_ComEnderecoDuplicadoEmOutroImovel_DeveLancarConflictException()
    {
        // Arrange
        const string propertyId = "property-1";
        var request = ValidUpdateRequest();
        var property = Property.Create(
            propertyId,
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua das Flores",
            "rua das flores",
            "100",
            ComplementType.None);
        var newCity = City.Create("city-2", "Contagem", "contagem", "MG");
        var otherProperty = Property.Create(
            "property-2",
            newCity.Id,
            newCity.Name,
            request.Neighborhood,
            "savassi",
            request.Street,
            "rua pernambuco",
            request.Number,
            request.ComplementType,
            request.ComplementValue,
            "apto 3",
            null);

        _propertyRepository.GetByIdAsync(propertyId, Arg.Any<CancellationToken>()).Returns(property);
        _cityRepository.GetByIdAsync(request.CityId, Arg.Any<CancellationToken>()).Returns(newCity);
        _textNormalizer.Normalize(request.Neighborhood).Returns("savassi");
        _textNormalizer.Normalize(request.Street).Returns("rua pernambuco");
        _textNormalizer.Normalize(request.ComplementValue!).Returns("apto 3");
        _propertyRepository.GetByUniqueAddressAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<ComplementType>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>()).Returns(otherProperty);

        // Act
        var result = await _sut.UpdateAsync(propertyId, request);

        // Assert
        result.ShouldBeFailure(ErrorKind.Conflict, "Já existe um imóvel cadastrado para este endereço.");

        await _propertyRepository.DidNotReceive().UpdateAsync(Arg.Any<Property>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_ComMesmoEnderecoDoProprioImovel_DevePermitirAtualizacao()
    {
        // Arrange
        const string propertyId = "property-1";
        var request = ValidUpdateRequest();
        var property = Property.Create(
            propertyId,
            "city-2",
            "Contagem",
            request.Neighborhood,
            "savassi",
            request.Street,
            "rua pernambuco",
            request.Number,
            request.ComplementType,
            request.ComplementValue,
            "apto 3",
            "IDX-1");
        var city = City.Create("city-2", "Contagem", "contagem", "MG");

        _propertyRepository.GetByIdAsync(propertyId, Arg.Any<CancellationToken>()).Returns(property);
        _cityRepository.GetByIdAsync(request.CityId, Arg.Any<CancellationToken>()).Returns(city);
        _textNormalizer.Normalize(request.Neighborhood).Returns("savassi");
        _textNormalizer.Normalize(request.Street).Returns("rua pernambuco");
        _textNormalizer.Normalize(request.ComplementValue!).Returns("apto 3");
        _propertyRepository.GetByUniqueAddressAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<ComplementType>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>()).Returns(property);

        // Act
        var result = await _sut.UpdateAsync(propertyId, request);

        // Assert
        await _propertyRepository.Received(1).UpdateAsync(property, Arg.Any<CancellationToken>());
        result.ShouldBeSuccess().Id.Should().Be(propertyId);
    }
}
