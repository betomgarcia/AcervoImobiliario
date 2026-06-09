using AcervoImobiliario.Domain.Enums;

namespace AcervoImobiliario.Application.DTOs.Properties;

public sealed record UpdatePropertyRequest(
    string CityId,
    string Neighborhood,
    string Street,
    string Number,
    ComplementType ComplementType,
    string? ComplementValue,
    string? CadastralIndex,
    bool IsActive);
