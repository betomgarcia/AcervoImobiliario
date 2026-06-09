using AcervoImobiliario.Application.Enums;
using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Domain.Entities;
using AcervoImobiliario.Infrastructure.Persistence;
using AcervoImobiliario.Infrastructure.Persistence.Mappers;
using MongoDB.Driver;

namespace AcervoImobiliario.Infrastructure.Repositories;

public sealed class PropertyHistoryRepository : IPropertyHistoryRepository
{
    private readonly MongoDbContext _context;

    public PropertyHistoryRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(PropertyHistory history, CancellationToken cancellationToken = default)
    {
        var document = PropertyHistoryDocumentMapper.ToDocument(history);
        await _context.PropertyHistories.InsertOneAsync(document, cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<PropertyHistory>> ListByPropertyIdAsync(
        string propertyId,
        HistorySortDirection sortDirection = HistorySortDirection.Desc,
        CancellationToken cancellationToken = default)
    {
        var query = _context.PropertyHistories.Find(history => history.PropertyId == propertyId);

        var documents = sortDirection == HistorySortDirection.Asc
            ? await query.SortBy(history => history.EventDate).ToListAsync(cancellationToken)
            : await query.SortByDescending(history => history.EventDate).ToListAsync(cancellationToken);

        return documents.Select(PropertyHistoryDocumentMapper.ToEntity).ToList();
    }
}
