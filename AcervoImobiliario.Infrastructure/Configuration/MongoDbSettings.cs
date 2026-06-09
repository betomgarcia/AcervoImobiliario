namespace AcervoImobiliario.Infrastructure.Configuration;

public sealed class MongoDbSettings
{
    public const string SectionName = "MongoDb";

    public string ConnectionString { get; init; } = "mongodb://localhost:27017";
    public string DatabaseName { get; init; } = "AcervoImobiliario";
    public string CitiesCollectionName { get; init; } = "cities";
    public string PropertiesCollectionName { get; init; } = "properties";
    public string PropertyHistoriesCollectionName { get; init; } = "property_histories";
}
