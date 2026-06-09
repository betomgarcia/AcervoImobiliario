using AcervoImobiliario.Domain.Enums;

namespace AcervoImobiliario.Application.DTOs.Properties;

public sealed record CreatePropertyRequest(
    string CityId,
    string Neighborhood,
    string Street,
    string Number,
    ComplementType ComplementType,
    string? ComplementValue,
    string? CadastralIndex);
