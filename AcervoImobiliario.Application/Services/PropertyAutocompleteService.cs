using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Application.Validators;

namespace AcervoImobiliario.Application.Services;

public sealed class PropertyAutocompleteService : IPropertyAutocompleteService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ITextNormalizer _textNormalizer;

    public PropertyAutocompleteService(
        IPropertyRepository propertyRepository,
        ITextNormalizer textNormalizer)
    {
        _propertyRepository = propertyRepository;
        _textNormalizer = textNormalizer;
    }

    public async Task<Result<IReadOnlyList<string>>> SearchNeighborhoodsAsync(
        SearchNeighborhoodsAutocompleteQuery query,
        CancellationToken cancellationToken = default)
    {
        var validation = SearchNeighborhoodsAutocompleteQueryValidator.Validate(query);
        if (!validation.IsSuccess)
        {
            return Result<IReadOnlyList<string>>.From(validation);
        }

        var termNormalized = _textNormalizer.Normalize(query.Term);
        var neighborhoods = await _propertyRepository.SearchDistinctNeighborhoodsAsync(
            query.CityId.Trim(),
            termNormalized,
            cancellationToken);

        return Result<IReadOnlyList<string>>.Success(neighborhoods);
    }

    public async Task<Result<IReadOnlyList<string>>> SearchStreetsAsync(
        SearchStreetsAutocompleteQuery query,
        CancellationToken cancellationToken = default)
    {
        var validation = SearchStreetsAutocompleteQueryValidator.Validate(query);
        if (!validation.IsSuccess)
        {
            return Result<IReadOnlyList<string>>.From(validation);
        }

        var neighborhoodNormalized = _textNormalizer.Normalize(query.Neighborhood);
        var termNormalized = _textNormalizer.Normalize(query.Term);

        var streets = await _propertyRepository.SearchDistinctStreetsAsync(
            query.CityId.Trim(),
            neighborhoodNormalized,
            termNormalized,
            cancellationToken);

        return Result<IReadOnlyList<string>>.Success(streets);
    }

    public async Task<Result<IReadOnlyList<string>>> SearchNumbersAsync(
        SearchNumbersAutocompleteQuery query,
        CancellationToken cancellationToken = default)
    {
        var validation = SearchNumbersAutocompleteQueryValidator.Validate(query);
        if (!validation.IsSuccess)
        {
            return Result<IReadOnlyList<string>>.From(validation);
        }

        var neighborhoodNormalized = _textNormalizer.Normalize(query.Neighborhood);
        var streetNormalized = _textNormalizer.Normalize(query.Street);
        var term = query.Term.Trim();

        var numbers = await _propertyRepository.SearchDistinctNumbersAsync(
            query.CityId.Trim(),
            neighborhoodNormalized,
            streetNormalized,
            term,
            cancellationToken);

        return Result<IReadOnlyList<string>>.Success(numbers);
    }
}
