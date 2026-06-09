namespace AcervoImobiliario.Application.DTOs.Properties;

public sealed record SearchNumbersAutocompleteQuery(
    string CityId,
    string Neighborhood,
    string Street,
    string Term);
