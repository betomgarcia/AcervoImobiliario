using AcervoImobiliario.Domain.Enums;

namespace AcervoImobiliario.Application.DTOs.Properties;

public sealed record SearchPropertiesQuery(
    string? CityId = null,
    string? Neighborhood = null,
    string? Street = null,
    string? Number = null,
    ComplementType? ComplementType = null,
    string? ComplementValue = null,
    string? CadastralIndex = null,
    bool IncludeInactive = false);
