namespace AcervoImobiliario.Application.DTOs.Properties;

public sealed record SearchPropertiesQuery(
    string? CityId = null,
    string? Neighborhood = null,
    string? Street = null,
    string? Number = null,
    string? Complement = null,
    string? CadastralIndex = null,
    bool IncludeInactive = false);
