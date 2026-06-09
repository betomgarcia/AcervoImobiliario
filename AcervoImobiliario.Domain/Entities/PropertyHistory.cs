using AcervoImobiliario.Domain.Enums;
using AcervoImobiliario.Domain.Exceptions;

namespace AcervoImobiliario.Domain.Entities;

/// <summary>
/// Representa um evento ocorrido no imóvel. Registros são imutáveis após criação.
/// </summary>
public sealed class PropertyHistory
{
    public string Id { get; }
    public string PropertyId { get; }
    public PropertyHistoryEventType EventType { get; }
    public DateTime EventDate { get; }
    public string Description { get; }
    public DateTime CreatedAt { get; }

    private PropertyHistory(
        string id,
        string propertyId,
        PropertyHistoryEventType eventType,
        DateTime eventDate,
        string description,
        DateTime createdAt)
    {
        Id = id;
        PropertyId = propertyId;
        EventType = eventType;
        EventDate = eventDate;
        Description = description;
        CreatedAt = createdAt;
    }

    public static PropertyHistory Restore(
        string id,
        string propertyId,
        PropertyHistoryEventType eventType,
        DateTime eventDate,
        string description,
        DateTime createdAt) =>
        new(id, propertyId, eventType, eventDate, description, createdAt);

    public static PropertyHistory Create(
        string id,
        string propertyId,
        PropertyHistoryEventType eventType,
        DateTime eventDate,
        string description)
    {
        ValidateId(id);
        ValidatePropertyId(propertyId);
        ValidateDescription(description);

        return new PropertyHistory(
            id.Trim(),
            propertyId.Trim(),
            eventType,
            eventDate.ToUniversalTime(),
            description.Trim(),
            DateTime.UtcNow);
    }

    private static void ValidateId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new DomainException("O identificador do histórico é obrigatório.");
        }
    }

    private static void ValidatePropertyId(string propertyId)
    {
        if (string.IsNullOrWhiteSpace(propertyId))
        {
            throw new DomainException("O identificador do imóvel é obrigatório.");
        }
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("A descrição do evento é obrigatória.");
        }
    }
}
