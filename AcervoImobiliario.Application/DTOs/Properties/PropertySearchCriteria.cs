namespace AcervoImobiliario.Application.DTOs.Properties;

public sealed class PropertySearchCriteria
{
    public string? CityId { get; init; }
    public string? NeighborhoodNormalized { get; init; }
    public string? StreetNormalized { get; init; }
    public string? Number { get; init; }
    public string? ComplementNormalized { get; init; }
    public string? CadastralIndex { get; init; }
    public bool ActiveOnly { get; init; } = true;
}
