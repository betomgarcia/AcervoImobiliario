using AcervoImobiliario.Domain.Enums;

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
    ComplementType ComplementType,
    string? ComplementValue,
    string? ComplementValueNormalized,
    string? CadastralIndex,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
