using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Cities;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.DTOs.PropertyHistories;
using AcervoImobiliario.Application.Factories;
using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Application.Services;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Application.Validators;
using AcervoImobiliario.Domain.Entities;
using AcervoImobiliario.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace AcervoImobiliario.Application.UnitTests.Rules;

/// <summary>
/// Suite canônica das principais regras de negócio do sistema.
/// </summary>
public class SystemRulesTests
{
    private readonly TextNormalizer _textNormalizer = new();
    private readonly AddressNormalizationService _addressNormalizationService = new();

    [Fact]
    [Trait("Regra", "Normalização de texto")]
    public void NormalizacaoDeTexto_DeveRemoverAcentosMinusculasEEspacosExtras()
    {
        var result = _textNormalizer.Normalize("  São   José  ");

        result.Should().Be("sao jose");
    }

    [Theory]
    [Trait("Regra", "Normalização de complemento")]
    [InlineData("Apartamento 303 Bloco A", "APT 303 BLOCO A")]
    [InlineData("APT 303 BLOCO A", "APT 303 BLOCO A")]
    [InlineData(" apto 303 bloco a ", "APT 303 BLOCO A")]
    public void NormalizacaoDeComplemento_DeveUnificarEscritasEquivalentes(
        string input,
        string expected)
    {
        _addressNormalizationService.NormalizeComplement(input).Should().Be(expected);
    }

    [Fact]
    [Trait("Regra", "Cidade duplicada")]
    public async Task CidadeDuplicada_DeveRetornarConflito()
    {
        var cityRepository = Substitute.For<ICityRepository>();
        var cityFactory = Substitute.For<ICityFactory>();
        var normalizer = Substitute.For<ITextNormalizer>();
        var sut = new CityService(cityRepository, cityFactory, normalizer);

        var request = new CreateCityRequest("Belo Horizonte", "MG");
        var existingCity = City.Create("city-1", "Belo Horizonte", "belo horizonte", "MG");

        normalizer.Normalize(request.Name).Returns("belo horizonte");
        cityRepository.GetByNameNormalizedAndStateAsync("belo horizonte", "MG", Arg.Any<CancellationToken>())
            .Returns(existingCity);

        var result = await sut.CreateAsync(request);

        result.ShouldBeFailure(
            ErrorKind.Conflict,
            "Já existe uma cidade cadastrada com este nome.");
        await cityRepository.DidNotReceive().CreateAsync(Arg.Any<City>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    [Trait("Regra", "Imóvel duplicado")]
    public async Task ImovelDuplicado_DeveRetornarConflito()
    {
        var propertyRepository = Substitute.For<IPropertyRepository>();
        var cityRepository = Substitute.For<ICityRepository>();
        var propertyFactory = Substitute.For<IPropertyFactory>();
        var normalizer = Substitute.For<ITextNormalizer>();
        var addressNormalization = Substitute.For<IAddressNormalizationService>();
        var sut = new PropertyService(
            propertyRepository,
            cityRepository,
            propertyFactory,
            normalizer,
            addressNormalization);

        var request = new CreatePropertyRequest(
            "city-1", "Centro", "Rua das Flores", "100", "Apto 12", null);
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
            request.Complement,
            "APT 12");

        cityRepository.GetByIdAsync(request.CityId, Arg.Any<CancellationToken>()).Returns(city);
        normalizer.Normalize(request.Neighborhood).Returns("centro");
        normalizer.Normalize(request.Street).Returns("rua das flores");
        addressNormalization.NormalizeComplement(request.Complement).Returns("APT 12");
        propertyRepository.GetByUniqueAddressAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            "APT 12",
            Arg.Any<CancellationToken>()).Returns(existingProperty);

        var result = await sut.CreateAsync(request);

        result.ShouldBeFailure(ErrorKind.Conflict, "Já existe um imóvel cadastrado para este endereço.");
        await propertyRepository.DidNotReceive().CreateAsync(Arg.Any<Property>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    [Trait("Regra", "Complemento equivalente na chave")]
    public void ComplementosEquivalentes_DevemGerarMesmaChaveDeEndereco()
    {
        var normalizedA = _addressNormalizationService.NormalizeComplement("Apartamento 303 Bloco A");
        var normalizedB = _addressNormalizationService.NormalizeComplement(" apto 303 bloco a ");

        var propertyA = Property.Create(
            "property-a",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua A",
            "rua a",
            "100",
            "Apartamento 303 Bloco A",
            normalizedA);

        var propertyB = Property.Create(
            "property-b",
            "city-1",
            "Belo Horizonte",
            "Centro",
            "centro",
            "Rua A",
            "rua a",
            "100",
            " apto 303 bloco a ",
            normalizedB);

        propertyA.GenerateAddressKey().Should().Be(propertyB.GenerateAddressKey());
    }

    [Theory]
    [Trait("Regra", "Número somente dígitos")]
    [InlineData("10A")]
    [InlineData("S/N")]
    [InlineData("12-34")]
    public void Numero_SomenteDigitos_DeveRejeitarValoresInvalidos(string number)
    {
        var result = PropertyAddressValidator.Validate(
            "city-1", "Centro", "Rua A", number);

        result.ShouldBeFailure(ErrorKind.Validation);
        result.Error!.Errors.Should().Contain("O número deve conter somente dígitos.");
    }

    [Theory]
    [Trait("Regra", "Busca inválida por number")]
    [InlineData(null, null, null, "100", "Não é permitido buscar somente por number.")]
    [InlineData("city-1", null, null, "100", "Não é permitido buscar por number sem cityId, neighborhood e street.")]
    [InlineData("city-1", "Centro", null, "100", "Não é permitido buscar por number sem cityId, neighborhood e street.")]
    public void BuscaPorNumber_SemHierarquiaCompleta_DeveFalhar(
        string? cityId,
        string? neighborhood,
        string? street,
        string number,
        string expectedMessage)
    {
        var query = new SearchPropertiesQuery(cityId, neighborhood, street, number);

        var result = SearchPropertiesQueryValidator.Validate(query);

        result.ShouldBeFailure(ErrorKind.Validation, expectedMessage);
    }

    [Fact]
    [Trait("Regra", "Histórico para imóvel existente")]
    public async Task Historico_ParaImovelExistente_DeveRegistrarComSucesso()
    {
        var historyRepository = Substitute.For<IPropertyHistoryRepository>();
        var propertyRepository = Substitute.For<IPropertyRepository>();
        var sut = new PropertyHistoryService(historyRepository, propertyRepository);

        const string propertyId = "property-1";
        var request = new CreatePropertyHistoryRequest(
            PropertyHistoryEventType.Note,
            DateTime.UtcNow,
            "Observação registrada");

        propertyRepository.GetByIdAsync(propertyId, Arg.Any<CancellationToken>())
            .Returns(Property.Create(
                propertyId, "city-1", "Belo Horizonte", "Centro", "centro", "Rua A", "rua a", "100"));

        var result = await sut.CreateAsync(propertyId, request);

        var response = result.ShouldBeSuccess();
        response.PropertyId.Should().Be(propertyId);
        await historyRepository.Received(1).CreateAsync(
            Arg.Is<PropertyHistory>(history => history.PropertyId == propertyId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    [Trait("Regra", "Histórico para imóvel inexistente")]
    public async Task Historico_ParaImovelInexistente_DeveRetornarNaoEncontrado()
    {
        var historyRepository = Substitute.For<IPropertyHistoryRepository>();
        var propertyRepository = Substitute.For<IPropertyRepository>();
        var sut = new PropertyHistoryService(historyRepository, propertyRepository);

        propertyRepository.GetByIdAsync("inexistente", Arg.Any<CancellationToken>())
            .Returns((Property?)null);

        var request = new CreatePropertyHistoryRequest(
            PropertyHistoryEventType.Note,
            DateTime.UtcNow,
            "Observação registrada");

        var result = await sut.CreateAsync("inexistente", request);

        result.ShouldBeFailure(ErrorKind.NotFound, "Imóvel 'inexistente' não encontrado.");
        await historyRepository.DidNotReceive().CreateAsync(
            Arg.Any<PropertyHistory>(),
            Arg.Any<CancellationToken>());
    }
}
