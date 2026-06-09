using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AcervoImobiliario.Infrastructure.Persistence.Documents;

public sealed class CityDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string NameNormalized { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
