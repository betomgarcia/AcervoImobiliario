namespace AcervoImobiliario.Application.DTOs.Properties;

public sealed record UpdatePropertyRequest(
    string CityId,
    string Neighborhood,
    string Street,
    string Number,
    string? Complement,
    string? CadastralIndex,
    bool IsActive);
