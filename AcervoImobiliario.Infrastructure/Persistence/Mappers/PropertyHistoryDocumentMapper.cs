using AcervoImobiliario.Domain.Entities;
using AcervoImobiliario.Infrastructure.Persistence.Documents;

namespace AcervoImobiliario.Infrastructure.Persistence.Mappers;

internal static class PropertyHistoryDocumentMapper
{
    public static PropertyHistoryDocument ToDocument(PropertyHistory history) =>
        new()
        {
            Id = history.Id,
            PropertyId = history.PropertyId,
            EventType = history.EventType,
            EventDate = history.EventDate,
            Description = history.Description,
            CreatedAt = history.CreatedAt
        };

    public static PropertyHistory ToEntity(PropertyHistoryDocument document) =>
        PropertyHistory.Restore(
            document.Id,
            document.PropertyId,
            document.EventType,
            document.EventDate,
            document.Description,
            document.CreatedAt);
}
