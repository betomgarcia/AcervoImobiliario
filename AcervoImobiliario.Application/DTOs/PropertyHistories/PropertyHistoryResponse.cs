using AcervoImobiliario.Domain.Enums;

namespace AcervoImobiliario.Application.DTOs.PropertyHistories;

public sealed record PropertyHistoryResponse(
    string Id,
    string PropertyId,
    PropertyHistoryEventType EventType,
    DateTime EventDate,
    string Description,
    DateTime CreatedAt);
