using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Application.Factories;
using AcervoImobiliario.Application.Mappings;
using AcervoImobiliario.Application.Validators;
using AcervoImobiliario.Domain.Entities;

namespace AcervoImobiliario.Application.Services;

public sealed class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IPropertyFactory _propertyFactory;
    private readonly ITextNormalizer _textNormalizer;
    private readonly IAddressNormalizationService _addressNormalizationService;

    public PropertyService(
        IPropertyRepository propertyRepository,
        ICityRepository cityRepository,
        IPropertyFactory propertyFactory,
        ITextNormalizer textNormalizer,
        IAddressNormalizationService addressNormalizationService)
    {
        _propertyRepository = propertyRepository;
        _cityRepository = cityRepository;
        _propertyFactory = propertyFactory;
        _textNormalizer = textNormalizer;
        _addressNormalizationService = addressNormalizationService;
    }

    public async Task<Result<IReadOnlyList<PropertyResponse>>> SearchAsync(
        SearchPropertiesQuery query,
        CancellationToken cancellationToken = default)
    {
        var validation = SearchPropertiesQueryValidator.Validate(query);
        if (!validation.IsSuccess)
        {
            return Result<IReadOnlyList<PropertyResponse>>.From(validation);
        }

        var criteria = BuildSearchCriteria(query);
        var properties = await _propertyRepository.SearchAsync(criteria, cancellationToken);

        return Result<IReadOnlyList<PropertyResponse>>.Success(
            properties.Select(PropertyMapper.ToResponse).ToList());
    }

    public async Task<Result<PropertyResponse>> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(id, cancellationToken);
        if (property is null)
        {
            return Result<PropertyResponse>.NotFound($"Imóvel '{id}' não encontrado.");
        }

        return Result<PropertyResponse>.Success(PropertyMapper.ToResponse(property));
    }

    public async Task<Result<PropertyResponse>> CreateAsync(
        CreatePropertyRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = CreatePropertyRequestValidator.Validate(request);
        if (!validation.IsSuccess)
        {
            return Result<PropertyResponse>.From(validation);
        }

        var city = await _cityRepository.GetByIdAsync(request.CityId, cancellationToken);
        if (city is null)
        {
            return Result<PropertyResponse>.NotFound($"Cidade '{request.CityId}' não encontrada.");
        }

        var cityValidation = ValidateCityForPropertyRegistration(city);
        if (!cityValidation.IsSuccess)
        {
            return Result<PropertyResponse>.From(cityValidation);
        }

        var uniqueness = await EnsureAddressIsUniqueAsync(
            request.CityId,
            request.Neighborhood,
            request.Street,
            request.Number,
            request.Complement,
            excludedPropertyId: null,
            cancellationToken);

        if (!uniqueness.IsSuccess)
        {
            return Result<PropertyResponse>.From(uniqueness);
        }

        var property = _propertyFactory.Create(
            Guid.NewGuid().ToString(),
            city.Id,
            city.Name,
            request.Neighborhood,
            request.Street,
            request.Number,
            request.Complement,
            request.CadastralIndex);

        await _propertyRepository.CreateAsync(property, cancellationToken);

        return Result<PropertyResponse>.Success(PropertyMapper.ToResponse(property));
    }

    public async Task<Result<PropertyResponse>> UpdateAsync(
        string id,
        UpdatePropertyRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = UpdatePropertyRequestValidator.Validate(request);
        if (!validation.IsSuccess)
        {
            return Result<PropertyResponse>.From(validation);
        }

        var property = await _propertyRepository.GetByIdAsync(id, cancellationToken);
        if (property is null)
        {
            return Result<PropertyResponse>.NotFound($"Imóvel '{id}' não encontrado.");
        }

        var city = await _cityRepository.GetByIdAsync(request.CityId, cancellationToken);
        if (city is null)
        {
            return Result<PropertyResponse>.NotFound($"Cidade '{request.CityId}' não encontrada.");
        }

        var cityValidation = ValidateCityForPropertyRegistration(city, property.CityId);
        if (!cityValidation.IsSuccess)
        {
            return Result<PropertyResponse>.From(cityValidation);
        }

        var uniqueness = await EnsureAddressIsUniqueAsync(
            request.CityId,
            request.Neighborhood,
            request.Street,
            request.Number,
            request.Complement,
            excludedPropertyId: property.Id,
            cancellationToken);

        if (!uniqueness.IsSuccess)
        {
            return Result<PropertyResponse>.From(uniqueness);
        }

        _propertyFactory.UpdateAddress(
            property,
            city.Id,
            city.Name,
            request.Neighborhood,
            request.Street,
            request.Number,
            request.Complement,
            request.CadastralIndex,
            request.IsActive);

        await _propertyRepository.UpdateAsync(property, cancellationToken);

        return Result<PropertyResponse>.Success(PropertyMapper.ToResponse(property));
    }

    private static Result ValidateCityForPropertyRegistration(City city, string? existingCityId = null)
    {
        if (city.IsActive)
        {
            return Result.Success();
        }

        if (!string.IsNullOrWhiteSpace(existingCityId) && city.Id == existingCityId)
        {
            return Result.Success();
        }

        return Result.ValidationFailure(
            "A cidade selecionada está inativa e não pode ser usada em novos cadastros.");
    }

    private async Task<Result> EnsureAddressIsUniqueAsync(
        string cityId,
        string neighborhood,
        string street,
        string number,
        string? complement,
        string? excludedPropertyId,
        CancellationToken cancellationToken)
    {
        var neighborhoodNormalized = _textNormalizer.Normalize(neighborhood);
        var streetNormalized = _textNormalizer.Normalize(street);
        var complementNormalized = NormalizeComplementForSearch(complement);

        var existingProperty = await _propertyRepository.GetByUniqueAddressAsync(
            cityId.Trim(),
            neighborhoodNormalized,
            streetNormalized,
            number.Trim(),
            complementNormalized,
            cancellationToken);

        if (existingProperty is not null && existingProperty.Id != excludedPropertyId)
        {
            return Result.Conflict("Já existe um imóvel cadastrado para este endereço.");
        }

        return Result.Success();
    }

    private PropertySearchCriteria BuildSearchCriteria(SearchPropertiesQuery query)
    {
        if (!string.IsNullOrWhiteSpace(query.CadastralIndex))
        {
            return new PropertySearchCriteria
            {
                CadastralIndex = query.CadastralIndex.Trim(),
                ActiveOnly = !query.IncludeInactive
            };
        }

        return new PropertySearchCriteria
        {
            CityId = query.CityId!.Trim(),
            NeighborhoodNormalized = string.IsNullOrWhiteSpace(query.Neighborhood)
                ? null
                : _textNormalizer.Normalize(query.Neighborhood),
            StreetNormalized = string.IsNullOrWhiteSpace(query.Street)
                ? null
                : _textNormalizer.Normalize(query.Street),
            Number = string.IsNullOrWhiteSpace(query.Number) ? null : query.Number.Trim(),
            ComplementNormalized = HasComplementFilter(query.Complement)
                ? NormalizeComplementForSearch(query.Complement)
                : null,
            ActiveOnly = !query.IncludeInactive
        };
    }

    private string NormalizeComplementForSearch(string? complement) =>
        string.IsNullOrWhiteSpace(complement)
            ? string.Empty
            : _addressNormalizationService.NormalizeComplement(complement);

    private static bool HasComplementFilter(string? complement) =>
        complement is not null;
}
