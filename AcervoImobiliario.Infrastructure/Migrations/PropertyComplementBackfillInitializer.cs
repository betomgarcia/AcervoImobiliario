using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Infrastructure.Configuration;
using AcervoImobiliario.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AcervoImobiliario.Infrastructure.Migrations;

public sealed class PropertyComplementBackfillInitializer
{
    private readonly MongoDbContext _context;
    private readonly IAddressNormalizationService _addressNormalizationService;
    private readonly ILogger<PropertyComplementBackfillInitializer> _logger;
    private readonly string _propertiesCollectionName;

    public PropertyComplementBackfillInitializer(
        MongoDbContext context,
        IAddressNormalizationService addressNormalizationService,
        IOptions<MongoDbSettings> settings,
        ILogger<PropertyComplementBackfillInitializer> logger)
    {
        _context = context;
        _addressNormalizationService = addressNormalizationService;
        _logger = logger;
        _propertiesCollectionName = settings.Value.PropertiesCollectionName;
    }

    public async Task BackfillAsync(CancellationToken cancellationToken = default)
    {
        var collection = _context.Database.GetCollection<BsonDocument>(_propertiesCollectionName);
        var filter = Builders<BsonDocument>.Filter.Or(
            Builders<BsonDocument>.Filter.Not(
                Builders<BsonDocument>.Filter.Exists("ComplementNormalized")),
            Builders<BsonDocument>.Filter.Eq("ComplementNormalized", BsonNull.Value),
            Builders<BsonDocument>.Filter.Eq("ComplementNormalized", string.Empty));

        var documents = await collection.Find(filter).ToListAsync(cancellationToken);
        if (documents.Count == 0)
        {
            _logger.LogDebug("Nenhum imóvel pendente de backfill de ComplementNormalized.");
            return;
        }

        var migratedCount = 0;

        foreach (var document in documents)
        {
            var complement = ResolveLegacyComplement(document);
            var complementNormalized = _addressNormalizationService.NormalizeComplement(complement);

            var update = Builders<BsonDocument>.Update
                .Set("Complement", complement is null ? BsonNull.Value : (BsonValue)complement)
                .Set("ComplementNormalized", complementNormalized)
                .Unset("ComplementType")
                .Unset("ComplementValue")
                .Unset("ComplementValueNormalized");

            await collection.UpdateOneAsync(
                Builders<BsonDocument>.Filter.Eq("_id", document["_id"]),
                update,
                cancellationToken: cancellationToken);

            migratedCount++;
        }

        _logger.LogInformation(
            "Backfill de ComplementNormalized concluído para {Count} imóvel(is).",
            migratedCount);
    }

    private static string? ResolveLegacyComplement(BsonDocument document)
    {
        var complement = GetOptionalString(document, "Complement");
        if (!string.IsNullOrWhiteSpace(complement))
        {
            return complement.Trim();
        }

        var complementValue = GetOptionalString(document, "ComplementValue");
        if (!string.IsNullOrWhiteSpace(complementValue))
        {
            return complementValue.Trim();
        }

        return null;
    }

    private static string? GetOptionalString(BsonDocument document, string fieldName)
    {
        if (!document.TryGetValue(fieldName, out var value) || value.IsBsonNull)
        {
            return null;
        }

        return value.ToString();
    }
}
