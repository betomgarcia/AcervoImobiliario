using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Domain.Entities;

namespace AcervoImobiliario.Application.Factories;

public sealed class PropertyFactory : IPropertyFactory
{
    private readonly ITextNormalizer _textNormalizer;
    private readonly IAddressNormalizationService _addressNormalizationService;

    public PropertyFactory(
        ITextNormalizer textNormalizer,
        IAddressNormalizationService addressNormalizationService)
    {
        _textNormalizer = textNormalizer;
        _addressNormalizationService = addressNormalizationService;
    }

    public Property Create(
        string id,
        string cityId,
        string cityNameSnapshot,
        string neighborhood,
        string street,
        string number,
        string? complement = null,
        string? cadastralIndex = null)
    {
        var (trimmedComplement, complementNormalized) = NormalizeComplementFields(complement);

        return Property.Create(
            id,
            cityId,
            cityNameSnapshot,
            neighborhood,
            _textNormalizer.Normalize(neighborhood),
            street,
            _textNormalizer.Normalize(street),
            number,
            trimmedComplement,
            complementNormalized,
            cadastralIndex);
    }

    public void UpdateAddress(
        Property property,
        string cityId,
        string cityNameSnapshot,
        string neighborhood,
        string street,
        string number,
        string? complement,
        string? cadastralIndex,
        bool isActive)
    {
        var (trimmedComplement, complementNormalized) = NormalizeComplementFields(complement);

        property.UpdateAddress(
            cityId,
            cityNameSnapshot,
            neighborhood,
            _textNormalizer.Normalize(neighborhood),
            street,
            _textNormalizer.Normalize(street),
            number,
            trimmedComplement,
            complementNormalized,
            cadastralIndex,
            isActive);
    }

    private (string? Complement, string ComplementNormalized) NormalizeComplementFields(string? complement)
    {
        if (string.IsNullOrWhiteSpace(complement))
        {
            return (null, string.Empty);
        }

        var trimmed = complement.Trim();
        var normalized = _addressNormalizationService.NormalizeComplement(trimmed);
        return string.IsNullOrEmpty(normalized) ? (null, string.Empty) : (trimmed, normalized);
    }
}
