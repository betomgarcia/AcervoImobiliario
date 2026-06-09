using AcervoImobiliario.Domain.Enums;

namespace AcervoImobiliario.Application.DTOs.PropertyHistories;

public sealed record CreatePropertyHistoryRequest(
    PropertyHistoryEventType EventType,
    DateTime EventDate,
    string Description);
