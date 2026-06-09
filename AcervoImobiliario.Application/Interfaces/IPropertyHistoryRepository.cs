using AcervoImobiliario.Application.Enums;
using AcervoImobiliario.Domain.Entities;

namespace AcervoImobiliario.Application.Interfaces;

public interface IPropertyHistoryRepository
{
    Task<IReadOnlyList<PropertyHistory>> ListByPropertyIdAsync(
        string propertyId,
        HistorySortDirection sortDirection = HistorySortDirection.Desc,
        CancellationToken cancellationToken = default);

    Task CreateAsync(PropertyHistory history, CancellationToken cancellationToken = default);
}
