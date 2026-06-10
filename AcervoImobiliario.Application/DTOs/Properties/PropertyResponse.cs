namespace AcervoImobiliario.Application.DTOs.Properties;

public sealed record PropertyResponse(
    string Id,
    string CityId,
    string CityNameSnapshot,
    string Neighborhood,
    string NeighborhoodNormalized,
    string Street,
    string StreetNormalized,
    string Number,
    string? Complement,
    string ComplementNormalized,
    string? CadastralIndex,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
