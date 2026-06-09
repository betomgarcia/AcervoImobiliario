using AcervoImobiliario.Domain.Entities;
using AcervoImobiliario.Domain.Enums;

namespace AcervoImobiliario.Application.Factories;

public interface IPropertyFactory
{
    Property Create(
        string id,
        string cityId,
        string cityNameSnapshot,
        string neighborhood,
        string street,
        string number,
        ComplementType complementType,
        string? complementValue = null,
        string? cadastralIndex = null);

    void UpdateAddress(
        Property property,
        string cityId,
        string cityNameSnapshot,
        string neighborhood,
        string street,
        string number,
        ComplementType complementType,
        string? complementValue,
        string? cadastralIndex,
        bool isActive);
}
