using AcervoImobiliario.Infrastructure.Persistence.Documents;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace AcervoImobiliario.Infrastructure.Persistence;

public sealed class MongoDbIndexInitializer
{
    private const string PropertyUniqueAddressIndexName = "ux_property_unique_address";

    private readonly MongoDbContext _context;
    private readonly ILogger<MongoDbIndexInitializer> _logger;

    public MongoDbIndexInitializer(MongoDbContext context, ILogger<MongoDbIndexInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var cityIndexModel = new CreateIndexModel<CityDocument>(
            Builders<CityDocument>.IndexKeys
                .Ascending(city => city.NameNormalized)
                .Ascending(city => city.State),
            new CreateIndexOptions
            {
                Unique = true,
                Name = "ux_city_name_normalized_state"
            });

        await _context.Cities.Indexes.CreateOneAsync(cityIndexModel, cancellationToken: cancellationToken);

        await DropPropertyUniqueAddressIndexIfExistsAsync(cancellationToken);

        var propertyIndexModel = new CreateIndexModel<PropertyDocument>(
            Builders<PropertyDocument>.IndexKeys
                .Ascending(property => property.CityId)
                .Ascending(property => property.NeighborhoodNormalized)
                .Ascending(property => property.StreetNormalized)
                .Ascending(property => property.Number)
                .Ascending(property => property.ComplementNormalized),
            new CreateIndexOptions
            {
                Unique = true,
                Name = PropertyUniqueAddressIndexName
            });

        await _context.Properties.Indexes.CreateOneAsync(propertyIndexModel, cancellationToken: cancellationToken);

        var propertyHistoryIndexModel = new CreateIndexModel<PropertyHistoryDocument>(
            Builders<PropertyHistoryDocument>.IndexKeys
                .Ascending(history => history.PropertyId)
                .Descending(history => history.EventDate),
            new CreateIndexOptions
            {
                Name = "ix_property_history_property_event_date"
            });

        await _context.PropertyHistories.Indexes.CreateOneAsync(
            propertyHistoryIndexModel,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Índices do MongoDB inicializados com sucesso.");
    }

    private async Task DropPropertyUniqueAddressIndexIfExistsAsync(CancellationToken cancellationToken)
    {
        var indexes = await _context.Properties.Indexes
            .ListAsync(cancellationToken);

        var indexNames = await indexes.ToListAsync(cancellationToken);
        var hasUniqueAddressIndex = indexNames.Any(index =>
            index.Contains("name") && index["name"].AsString == PropertyUniqueAddressIndexName);

        if (!hasUniqueAddressIndex)
        {
            return;
        }

        await _context.Properties.Indexes.DropOneAsync(PropertyUniqueAddressIndexName, cancellationToken);
        _logger.LogInformation("Índice {IndexName} removido para recriação.", PropertyUniqueAddressIndexName);
    }
}
