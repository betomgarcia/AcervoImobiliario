using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.Factories;
using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Application.Services;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace AcervoImobiliario.Application.UnitTests.Services;

public class PropertyServiceTests
{
    private readonly IPropertyRepository _propertyRepository = Substitute.For<IPropertyRepository>();
    private readonly ICityRepository _cityRepository = Substitute.For<ICityRepository>();
    private readonly IPropertyFactory _propertyFactory = Substitute.For<IPropertyFactory>();
    private readonly ITextNormalizer _textNormalizer = Substitute.For<ITextNormalizer>();
    private readonly IAddressNormalizationService _addressNormalizationService =
        Substitute.For<IAddressNormalizationService>();
    private readonly PropertyService _sut;

    public PropertyServiceTests()
    {
        _sut = new PropertyService(
            _propertyRepository,
            _cityRepository,
            _propertyFactory,
            _textNormalizer,
            _addressNormalizationService);
    }

    private static CreatePropertyRequest ValidRequest() =>
        new("city-1", "Centro", "Rua das Flores", "100", "Apto 12", "IDX-1");

    [Fact]
    public async Task SearchAsync_ComCityId_DeveBuscarComFiltroAtivoPorPadrao()
    {
        var query = new SearchPropertiesQuery(CityId: "city-1");
        var property = Property.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua A",
            "rua a",
            "100");

        _propertyRepository.SearchAsync(
            Arg.Is<PropertySearchCriteria>(criteria =>
                criteria.CityId == "city-1"
                && criteria.ActiveOnly
                && criteria.NeighborhoodNormalized == null),
            Arg.Any<CancellationToken>()).Returns(new List<Property> { property });

        var result = await _sut.SearchAsync(query);

        var properties = result.ShouldBeSuccess();
        properties.Should().ContainSingle();
        properties[0].Id.Should().Be("property-1");
    }

    [Fact]
    public async Task SearchAsync_ComEnderecoCompleto_DeveNormalizarCamposTextuais()
    {
        var query = new SearchPropertiesQuery(
            "city-1",
            "  Centro ",
            "  Rua das Flores ",
            "100",
            "  Apto 12 ");

        _textNormalizer.Normalize("  Centro ").Returns("centro");
        _textNormalizer.Normalize("  Rua das Flores ").Returns("rua das flores");
        _addressNormalizationService.NormalizeComplement("  Apto 12 ").Returns("APT 12");
        _propertyRepository.SearchAsync(Arg.Any<PropertySearchCriteria>(), Arg.Any<CancellationToken>())
            .Returns(new List<Property>());

        await _sut.SearchAsync(query);

        await _propertyRepository.Received(1).SearchAsync(
            Arg.Is<PropertySearchCriteria>(criteria =>
                criteria.NeighborhoodNormalized == "centro"
                && criteria.StreetNormalized == "rua das flores"
                && criteria.ComplementNormalized == "APT 12"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchAsync_ComCadastralIndex_DeveBuscarPorIndiceCadastral()
    {
        var query = new SearchPropertiesQuery(CadastralIndex: " IDX-1 ");
        _propertyRepository.SearchAsync(Arg.Any<PropertySearchCriteria>(), Arg.Any<CancellationToken>())
            .Returns(new List<Property>());

        await _sut.SearchAsync(query);

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
        var query = new SearchPropertiesQuery(CityId: "city-1", IncludeInactive: true);
        _propertyRepository.SearchAsync(Arg.Any<PropertySearchCriteria>(), Arg.Any<CancellationToken>())
            .Returns(new List<Property>());

        await _sut.SearchAsync(query);

        await _propertyRepository.Received(1).SearchAsync(
            Arg.Is<PropertySearchCriteria>(criteria => !criteria.ActiveOnly),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchAsync_ComCombinacaoInvalida_DeveRetornarErroDeValidacao()
    {
        var query = new SearchPropertiesQuery(Number: "100");

        var result = await _sut.SearchAsync(query);

        result.ShouldBeFailure(ErrorKind.Validation);
        await _propertyRepository.DidNotReceive().SearchAsync(
            Arg.Any<PropertySearchCriteria>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ComDadosValidos_DeveCriarImovel()
    {
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
            request.Complement,
            "APT 12",
            request.CadastralIndex);

        _cityRepository.GetByIdAsync(request.CityId, Arg.Any<CancellationToken>()).Returns(city);
        _textNormalizer.Normalize(request.Neighborhood).Returns("centro");
        _textNormalizer.Normalize(request.Street).Returns("rua das flores");
        _addressNormalizationService.NormalizeComplement(request.Complement).Returns("APT 12");
        _propertyRepository.GetByUniqueAddressAsync(
            city.Id,
            "centro",
            "rua das flores",
            request.Number,
            "APT 12",
            Arg.Any<CancellationToken>()).Returns((Property?)null);
        _propertyFactory.Create(
            Arg.Any<string>(),
            city.Id,
            city.Name,
            request.Neighborhood,
            request.Street,
            request.Number,
            request.Complement,
            request.CadastralIndex).Returns(property);

        var result = await _sut.CreateAsync(request);

        var response = result.ShouldBeSuccess();
        response.Id.Should().Be("property-1");
        await _propertyRepository.Received(1).CreateAsync(property, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ComEnderecosEquivalentesNormalizados_DeveDetectarDuplicidade()
    {
        var request = ValidRequest() with { Complement = "Apartamento 12" };
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
            "Apto 12",
            "APT 12");

        _cityRepository.GetByIdAsync(request.CityId, Arg.Any<CancellationToken>()).Returns(city);
        _textNormalizer.Normalize(request.Neighborhood).Returns("centro");
        _textNormalizer.Normalize(request.Street).Returns("rua das flores");
        _addressNormalizationService.NormalizeComplement(request.Complement).Returns("APT 12");
        _propertyRepository.GetByUniqueAddressAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            "APT 12",
            Arg.Any<CancellationToken>()).Returns(existingProperty);

        var result = await _sut.CreateAsync(request);

        result.ShouldBeFailure(ErrorKind.Conflict, "Já existe um imóvel cadastrado para este endereço.");
    }

    [Fact]
    public async Task CreateAsync_ComCidadeInexistente_DeveRetornarNotFound()
    {
        var request = ValidRequest();
        _cityRepository.GetByIdAsync(request.CityId, Arg.Any<CancellationToken>())
            .Returns((City?)null);

        var result = await _sut.CreateAsync(request);

        result.ShouldBeFailure(ErrorKind.NotFound, $"Cidade '{request.CityId}' não encontrada.");
    }

    [Fact]
    public async Task CreateAsync_ComCidadeInativa_DeveRetornarErroDeValidacao()
    {
        var request = ValidRequest();
        var city = City.Create("city-1", "Belo Horizonte", "belo horizonte", "MG");
        city.Deactivate();
        _cityRepository.GetByIdAsync(request.CityId, Arg.Any<CancellationToken>()).Returns(city);

        var result = await _sut.CreateAsync(request);

        result.ShouldBeFailure(
            ErrorKind.Validation,
            "A cidade selecionada está inativa e não pode ser usada em novos cadastros.");

        await _propertyRepository.DidNotReceive().CreateAsync(Arg.Any<Property>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_ComImovelExistente_DeveRetornarImovel()
    {
        var property = Property.Create(
            "property-1",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua das Flores",
            "rua das flores",
            "100");

        _propertyRepository.GetByIdAsync("property-1", Arg.Any<CancellationToken>()).Returns(property);

        var result = await _sut.GetByIdAsync("property-1");

        result.ShouldBeSuccess().Id.Should().Be("property-1");
    }

    [Fact]
    public async Task GetByIdAsync_ComImovelInexistente_DeveRetornarNotFound()
    {
        _propertyRepository.GetByIdAsync("inexistente", Arg.Any<CancellationToken>())
            .Returns((Property?)null);

        var result = await _sut.GetByIdAsync("inexistente");

        result.ShouldBeFailure(ErrorKind.NotFound, "Imóvel 'inexistente' não encontrado.");
    }

    private static UpdatePropertyRequest ValidUpdateRequest() =>
        new("city-2", "Savassi", "Rua Pernambuco", "200", "Apto 3", "IDX-2", false);

    [Fact]
    public async Task UpdateAsync_ComDadosValidos_DeveAtualizarImovel()
    {
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
            "Apto 12",
            "APT 12",
            "IDX-1");
        var newCity = City.Create("city-2", "Contagem", "contagem", "MG");

        _propertyRepository.GetByIdAsync(propertyId, Arg.Any<CancellationToken>()).Returns(property);
        _cityRepository.GetByIdAsync(request.CityId, Arg.Any<CancellationToken>()).Returns(newCity);
        _textNormalizer.Normalize(request.Neighborhood).Returns("savassi");
        _textNormalizer.Normalize(request.Street).Returns("rua pernambuco");
        _addressNormalizationService.NormalizeComplement(request.Complement).Returns("APT 3");
        _propertyRepository.GetByUniqueAddressAsync(
            newCity.Id,
            "savassi",
            "rua pernambuco",
            request.Number,
            "APT 3",
            Arg.Any<CancellationToken>()).Returns((Property?)null);

        var result = await _sut.UpdateAsync(propertyId, request);

        _propertyFactory.Received(1).UpdateAddress(
            property,
            newCity.Id,
            newCity.Name,
            request.Neighborhood,
            request.Street,
            request.Number,
            request.Complement,
            request.CadastralIndex,
            request.IsActive);
        await _propertyRepository.Received(1).UpdateAsync(property, Arg.Any<CancellationToken>());
        result.ShouldBeSuccess().Id.Should().Be(propertyId);
    }
}
