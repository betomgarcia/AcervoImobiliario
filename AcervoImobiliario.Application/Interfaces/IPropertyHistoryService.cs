using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.PropertyHistories;

namespace AcervoImobiliario.Application.Interfaces;

public interface IPropertyHistoryService
{
    Task<Result<PropertyHistoryResponse>> CreateAsync(
        string propertyId,
        CreatePropertyHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<PropertyHistoryResponse>>> ListByPropertyIdAsync(
        string propertyId,
        string? sortDirection = null,
        CancellationToken cancellationToken = default);
}
