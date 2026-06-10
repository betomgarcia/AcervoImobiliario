namespace AcervoImobiliario.Application.DTOs.Cities;

public sealed record ListCitiesQuery(string? Name, CityStatusFilter Status = CityStatusFilter.Active);
