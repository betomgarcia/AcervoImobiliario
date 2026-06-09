using AcervoImobiliario.Domain.Entities;
using AcervoImobiliario.Infrastructure.Persistence.Documents;

namespace AcervoImobiliario.Infrastructure.Persistence.Mappers;

internal static class CityDocumentMapper
{
    public static CityDocument ToDocument(City city) =>
        new()
        {
            Id = city.Id,
            Name = city.Name,
            NameNormalized = city.NameNormalized,
            State = city.State,
            IsActive = city.IsActive,
            CreatedAt = city.CreatedAt,
            UpdatedAt = city.UpdatedAt
        };

    public static City ToEntity(CityDocument document) =>
        City.Restore(
            document.Id,
            document.Name,
            document.NameNormalized,
            document.State,
            document.IsActive,
            document.CreatedAt,
            document.UpdatedAt);
}
