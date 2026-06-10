namespace AcervoImobiliario.Application.DTOs.Properties;

public sealed record CreatePropertyRequest(
    string CityId,
    string Neighborhood,
    string Street,
    string Number,
    string? Complement,
    string? CadastralIndex);
