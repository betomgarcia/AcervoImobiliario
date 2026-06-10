using AcervoImobiliario.Domain.Entities;
using AcervoImobiliario.Infrastructure.Persistence.Documents;

namespace AcervoImobiliario.Infrastructure.Persistence.Mappers;

internal static class PropertyDocumentMapper
{
    public static PropertyDocument ToDocument(Property property) =>
        new()
        {
            Id = property.Id,
            CityId = property.CityId,
            CityNameSnapshot = property.CityNameSnapshot,
            Neighborhood = property.Neighborhood,
            NeighborhoodNormalized = property.NeighborhoodNormalized,
            Street = property.Street,
            StreetNormalized = property.StreetNormalized,
            Number = property.Number,
            Complement = property.Complement,
            ComplementNormalized = property.ComplementNormalized,
            CadastralIndex = property.CadastralIndex,
            IsActive = property.IsActive,
            CreatedAt = property.CreatedAt,
            UpdatedAt = property.UpdatedAt
        };

    public static Property ToEntity(PropertyDocument document) =>
        Property.Restore(
            document.Id,
            document.CityId,
            document.CityNameSnapshot,
            document.Neighborhood,
            document.NeighborhoodNormalized,
            document.Street,
            document.StreetNormalized,
            document.Number,
            document.Complement,
            document.ComplementNormalized,
            document.CadastralIndex,
            document.IsActive,
            document.CreatedAt,
            document.UpdatedAt);
}
