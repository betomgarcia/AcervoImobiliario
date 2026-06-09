using AcervoImobiliario.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AcervoImobiliario.Infrastructure.Persistence.Documents;

public sealed class PropertyHistoryDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    public string PropertyId { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.String)]
    public PropertyHistoryEventType EventType { get; set; }

    public DateTime EventDate { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
