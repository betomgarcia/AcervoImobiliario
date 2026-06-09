using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.PropertyHistories;
using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Application.Mappings;
using AcervoImobiliario.Application.Validators;
using AcervoImobiliario.Domain.Entities;

namespace AcervoImobiliario.Application.Services;

public sealed class PropertyHistoryService : IPropertyHistoryService
{
    private readonly IPropertyHistoryRepository _historyRepository;
    private readonly IPropertyRepository _propertyRepository;

    public PropertyHistoryService(
        IPropertyHistoryRepository historyRepository,
        IPropertyRepository propertyRepository)
    {
        _historyRepository = historyRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<Result<PropertyHistoryResponse>> CreateAsync(
        string propertyId,
        CreatePropertyHistoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = CreatePropertyHistoryRequestValidator.Validate(request);
        if (!validation.IsSuccess)
        {
            return Result<PropertyHistoryResponse>.From(validation);
        }

        var property = await _propertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property is null)
        {
            return Result<PropertyHistoryResponse>.NotFound($"Imóvel '{propertyId}' não encontrado.");
        }

        var historyResult = DomainResult.Execute(() => PropertyHistory.Create(
            Guid.NewGuid().ToString(),
            propertyId,
            request.EventType,
            request.EventDate,
            request.Description));

        if (!historyResult.IsSuccess)
        {
            return Result<PropertyHistoryResponse>.From(historyResult);
        }

        await _historyRepository.CreateAsync(historyResult.Value!, cancellationToken);

        return Result<PropertyHistoryResponse>.Success(PropertyHistoryMapper.ToResponse(historyResult.Value!));
    }

    public async Task<Result<IReadOnlyList<PropertyHistoryResponse>>> ListByPropertyIdAsync(
        string propertyId,
        string? sortDirection = null,
        CancellationToken cancellationToken = default)
    {
        var sortResult = ListPropertyHistoriesQueryValidator.ParseSortDirection(sortDirection);
        if (!sortResult.IsSuccess)
        {
            return Result<IReadOnlyList<PropertyHistoryResponse>>.From(sortResult);
        }

        var property = await _propertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property is null)
        {
            return Result<IReadOnlyList<PropertyHistoryResponse>>.NotFound(
                $"Imóvel '{propertyId}' não encontrado.");
        }

        var histories = await _historyRepository.ListByPropertyIdAsync(
            propertyId,
            sortResult.Value!,
            cancellationToken);

        return Result<IReadOnlyList<PropertyHistoryResponse>>.Success(
            histories.Select(PropertyHistoryMapper.ToResponse).ToList());
    }
}
