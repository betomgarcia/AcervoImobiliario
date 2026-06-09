using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Domain.Entities;
using AcervoImobiliario.Domain.Enums;

namespace AcervoImobiliario.Application.Factories;

public sealed class PropertyFactory : IPropertyFactory
{
    private readonly ITextNormalizer _textNormalizer;

    public PropertyFactory(ITextNormalizer textNormalizer)
    {
        _textNormalizer = textNormalizer;
    }

    public Property Create(
        string id,
        string cityId,
        string cityNameSnapshot,
        string neighborhood,
        string street,
        string number,
        ComplementType complementType,
        string? complementValue = null,
        string? cadastralIndex = null)
    {
        return Property.Create(
            id,
            cityId,
            cityNameSnapshot,
            neighborhood,
            _textNormalizer.Normalize(neighborhood),
            street,
            _textNormalizer.Normalize(street),
            number,
            complementType,
            complementValue,
            NormalizeComplementValue(complementType, complementValue),
            cadastralIndex);
    }

    public void UpdateAddress(
        Property property,
        string cityId,
        string cityNameSnapshot,
        string neighborhood,
        string street,
        string number,
        ComplementType complementType,
        string? complementValue,
        string? cadastralIndex,
        bool isActive)
    {
        property.UpdateAddress(
            cityId,
            cityNameSnapshot,
            neighborhood,
            _textNormalizer.Normalize(neighborhood),
            street,
            _textNormalizer.Normalize(street),
            number,
            complementType,
            complementValue,
            NormalizeComplementValue(complementType, complementValue),
            cadastralIndex,
            isActive);
    }

    private string? NormalizeComplementValue(ComplementType complementType, string? complementValue)
    {
        if (complementType == ComplementType.None || string.IsNullOrWhiteSpace(complementValue))
        {
            return null;
        }

        var normalized = _textNormalizer.Normalize(complementValue);
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
