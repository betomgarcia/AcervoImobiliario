using AcervoImobiliario.Infrastructure.Migrations;
using AcervoImobiliario.Infrastructure.Seeding;

namespace AcervoImobiliario.Infrastructure.Persistence;

public sealed class MongoDbInitializer
{
    private readonly MongoDbIndexInitializer _indexInitializer;
    private readonly PropertyComplementBackfillInitializer _propertyComplementBackfillInitializer;
    private readonly CitySeedInitializer _citySeedInitializer;

    public MongoDbInitializer(
        MongoDbIndexInitializer indexInitializer,
        PropertyComplementBackfillInitializer propertyComplementBackfillInitializer,
        CitySeedInitializer citySeedInitializer)
    {
        _indexInitializer = indexInitializer;
        _propertyComplementBackfillInitializer = propertyComplementBackfillInitializer;
        _citySeedInitializer = citySeedInitializer;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _propertyComplementBackfillInitializer.BackfillAsync(cancellationToken);
        await _indexInitializer.InitializeAsync(cancellationToken);
        await _citySeedInitializer.SeedAsync(cancellationToken);
    }
}
