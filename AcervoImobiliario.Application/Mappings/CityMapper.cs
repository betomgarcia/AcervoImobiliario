using AcervoImobiliario.Application.DTOs.Cities;
using AcervoImobiliario.Domain.Entities;

namespace AcervoImobiliario.Application.Mappings;

internal static class CityMapper
{
    public static CityResponse ToResponse(City city) =>
        new(
            city.Id,
            city.Name,
            city.NameNormalized,
            city.State,
            city.IsActive,
            city.CreatedAt,
            city.UpdatedAt);
}
