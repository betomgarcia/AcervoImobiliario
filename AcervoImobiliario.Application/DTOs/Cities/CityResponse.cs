namespace AcervoImobiliario.Application.DTOs.Cities;

public sealed record CityResponse(
    string Id,
    string Name,
    string NameNormalized,
    string State,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
