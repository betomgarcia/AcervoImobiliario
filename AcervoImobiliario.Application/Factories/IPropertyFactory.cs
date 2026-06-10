using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Domain.Entities;

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
        string? complement = null,
        string? cadastralIndex = null);

    void UpdateAddress(
        Property property,
        string cityId,
        string cityNameSnapshot,
        string neighborhood,
        string street,
        string number,
        string? complement,
        string? cadastralIndex,
        bool isActive);
}
