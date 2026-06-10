using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Domain.Entities;

namespace AcervoImobiliario.Application.Mappings;

internal static class PropertyMapper
{
    public static PropertyResponse ToResponse(Property property) =>
        new(
            property.Id,
            property.CityId,
            property.CityNameSnapshot,
            property.Neighborhood,
            property.NeighborhoodNormalized,
            property.Street,
            property.StreetNormalized,
            property.Number,
            property.Complement,
            property.ComplementNormalized,
            property.CadastralIndex,
            property.IsActive,
            property.CreatedAt,
            property.UpdatedAt);
}
