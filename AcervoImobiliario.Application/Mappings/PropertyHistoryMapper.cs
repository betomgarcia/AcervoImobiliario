using AcervoImobiliario.Application.DTOs.PropertyHistories;
using AcervoImobiliario.Domain.Entities;

namespace AcervoImobiliario.Application.Mappings;

internal static class PropertyHistoryMapper
{
    public static PropertyHistoryResponse ToResponse(PropertyHistory history) =>
        new(
            history.Id,
            history.PropertyId,
            history.EventType,
            history.EventDate,
            history.Description,
            history.CreatedAt);
}
