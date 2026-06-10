using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AcervoImobiliario.Infrastructure.Persistence.Documents;

public sealed class PropertyDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    public string CityId { get; set; } = string.Empty;

    public string CityNameSnapshot { get; set; } = string.Empty;

    public string Neighborhood { get; set; } = string.Empty;

    public string NeighborhoodNormalized { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public string StreetNormalized { get; set; } = string.Empty;

    public string Number { get; set; } = string.Empty;

    public string? Complement { get; set; }

    public string ComplementNormalized { get; set; } = string.Empty;

    public string? CadastralIndex { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
