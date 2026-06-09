using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Cities;
using AcervoImobiliario.Application.Factories;
using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Application.Mappings;
using AcervoImobiliario.Application.Validators;

namespace AcervoImobiliario.Application.Services;

public sealed class CityService : ICityService
{
    private readonly ICityRepository _cityRepository;
    private readonly ICityFactory _cityFactory;
    private readonly ITextNormalizer _textNormalizer;

    public CityService(
        ICityRepository cityRepository,
        ICityFactory cityFactory,
        ITextNormalizer textNormalizer)
    {
        _cityRepository = cityRepository;
        _cityFactory = cityFactory;
        _textNormalizer = textNormalizer;
    }

    public async Task<Result<IReadOnlyList<CityResponse>>> ListActiveAsync(
        CancellationToken cancellationToken = default)
    {
        var cities = await _cityRepository.ListActiveAsync(cancellationToken);
        return Result<IReadOnlyList<CityResponse>>.Success(cities.Select(CityMapper.ToResponse).ToList());
    }

    public async Task<Result<IReadOnlyList<CityResponse>>> SearchAsync(
        string term,
        CancellationToken cancellationToken = default)
    {
        var validation = SearchCitiesQueryValidator.Validate(term);
        if (!validation.IsSuccess)
        {
            return Result<IReadOnlyList<CityResponse>>.From(validation);
        }

        var termNormalized = _textNormalizer.Normalize(term);
        var cities = await _cityRepository.SearchByNameNormalizedAsync(termNormalized, cancellationToken);

        return Result<IReadOnlyList<CityResponse>>.Success(cities.Select(CityMapper.ToResponse).ToList());
    }

    public async Task<Result<CityResponse>> CreateAsync(
        CreateCityRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = CreateCityRequestValidator.Validate(request);
        if (!validation.IsSuccess)
        {
            return Result<CityResponse>.From(validation);
        }

        var state = request.State.Trim().ToUpperInvariant();
        var nameNormalized = _textNormalizer.Normalize(request.Name);

        var existingCity = await _cityRepository.GetByNameNormalizedAndStateAsync(
            nameNormalized,
            state,
            cancellationToken);

        if (existingCity is not null)
        {
            return Result<CityResponse>.Conflict(
                $"Já existe uma cidade cadastrada com o nome '{request.Name.Trim()}' no estado '{state}'.");
        }

        var city = _cityFactory.Create(Guid.NewGuid().ToString(), request.Name, state);
        await _cityRepository.CreateAsync(city, cancellationToken);

        return Result<CityResponse>.Success(CityMapper.ToResponse(city));
    }

    public async Task<Result<CityResponse>> UpdateAsync(
        string id,
        UpdateCityRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = UpdateCityRequestValidator.Validate(request);
        if (!validation.IsSuccess)
        {
            return Result<CityResponse>.From(validation);
        }

        var city = await _cityRepository.GetByIdAsync(id, cancellationToken);
        if (city is null)
        {
            return Result<CityResponse>.NotFound($"Cidade '{id}' não encontrada.");
        }

        var state = request.State.Trim().ToUpperInvariant();
        var nameNormalized = _textNormalizer.Normalize(request.Name);

        var cityWithSameKey = await _cityRepository.GetByNameNormalizedAndStateAsync(
            nameNormalized,
            state,
            cancellationToken);

        if (cityWithSameKey is not null && cityWithSameKey.Id != city.Id)
        {
            return Result<CityResponse>.Conflict(
                $"Já existe outra cidade cadastrada com o nome '{request.Name.Trim()}' no estado '{state}'.");
        }

        _cityFactory.Update(city, request.Name, state, request.IsActive);
        await _cityRepository.UpdateAsync(city, cancellationToken);

        return Result<CityResponse>.Success(CityMapper.ToResponse(city));
    }
}
