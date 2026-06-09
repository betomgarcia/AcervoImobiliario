using AcervoImobiliario.Infrastructure.Seeding;

namespace AcervoImobiliario.Infrastructure.Persistence;

public sealed class MongoDbInitializer
{
    private readonly MongoDbIndexInitializer _indexInitializer;
    private readonly CitySeedInitializer _citySeedInitializer;

    public MongoDbInitializer(
        MongoDbIndexInitializer indexInitializer,
        CitySeedInitializer citySeedInitializer)
    {
        _indexInitializer = indexInitializer;
        _citySeedInitializer = citySeedInitializer;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _indexInitializer.InitializeAsync(cancellationToken);
        await _citySeedInitializer.SeedAsync(cancellationToken);
    }
}
