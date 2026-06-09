namespace AcervoImobiliario.Application.DTOs.Cities;

public sealed record UpdateCityRequest(string Name, string State, bool IsActive);
