using AcervoImobiliario.Infrastructure.Configuration;
using AcervoImobiliario.Infrastructure.Persistence.Documents;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AcervoImobiliario.Infrastructure.Persistence;

public sealed class MongoDbContext
{
    public IMongoDatabase Database { get; }
    public IMongoCollection<CityDocument> Cities { get; }
    public IMongoCollection<PropertyDocument> Properties { get; }
    public IMongoCollection<PropertyHistoryDocument> PropertyHistories { get; }

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var mongoSettings = settings.Value;
        var client = new MongoClient(mongoSettings.ConnectionString);
        Database = client.GetDatabase(mongoSettings.DatabaseName);

        Cities = Database.GetCollection<CityDocument>(mongoSettings.CitiesCollectionName);
        Properties = Database.GetCollection<PropertyDocument>(mongoSettings.PropertiesCollectionName);
        PropertyHistories = Database.GetCollection<PropertyHistoryDocument>(
            mongoSettings.PropertyHistoriesCollectionName);
    }
}
