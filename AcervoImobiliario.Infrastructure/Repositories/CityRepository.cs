using AcervoImobiliario.Application.DTOs.Cities;
using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Domain.Entities;
using AcervoImobiliario.Infrastructure.Persistence;
using AcervoImobiliario.Infrastructure.Persistence.Documents;
using AcervoImobiliario.Infrastructure.Persistence.Mappers;
using MongoDB.Driver;

namespace AcervoImobiliario.Infrastructure.Repositories;

public sealed class CityRepository : ICityRepository
{
    private readonly MongoDbContext _context;

    public CityRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(City city, CancellationToken cancellationToken = default)
    {
        var document = CityDocumentMapper.ToDocument(city);
        await _context.Cities.InsertOneAsync(document, cancellationToken: cancellationToken);
    }

    public async Task<City?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var document = await _context.Cities
            .Find(city => city.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return document is null ? null : CityDocumentMapper.ToEntity(document);
    }

    public async Task<City?> GetByNameNormalizedAndStateAsync(
        string nameNormalized,
        string state,
        CancellationToken cancellationToken = default)
    {
        var document = await _context.Cities
            .Find(city => city.NameNormalized == nameNormalized && city.State == state)
            .FirstOrDefaultAsync(cancellationToken);

        return document is null ? null : CityDocumentMapper.ToEntity(document);
    }

    public async Task<IReadOnlyList<City>> ListAsync(
        string? nameNormalized,
        CityStatusFilter status,
        CancellationToken cancellationToken = default)
    {
        var filter = BuildStatusFilter(status);

        if (!string.IsNullOrWhiteSpace(nameNormalized))
        {
            filter = Builders<CityDocument>.Filter.And(
                filter,
                Builders<CityDocument>.Filter.Regex(
                    city => city.NameNormalized,
                    new MongoDB.Bson.BsonRegularExpression(nameNormalized, "i")));
        }

        var documents = await _context.Cities
            .Find(filter)
            .SortBy(city => city.NameNormalized)
            .ToListAsync(cancellationToken);

        return documents.Select(CityDocumentMapper.ToEntity).ToList();
    }

    public Task<IReadOnlyList<City>> ListActiveAsync(CancellationToken cancellationToken = default) =>
        ListAsync(null, CityStatusFilter.Active, cancellationToken);

    public async Task<IReadOnlyList<City>> SearchActiveByNameNormalizedAsync(
        string termNormalized,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<CityDocument>.Filter.And(
            Builders<CityDocument>.Filter.Eq(city => city.IsActive, true),
            Builders<CityDocument>.Filter.Regex(
                city => city.NameNormalized,
                new MongoDB.Bson.BsonRegularExpression(termNormalized, "i")));

        var documents = await _context.Cities
            .Find(filter)
            .SortBy(city => city.NameNormalized)
            .ToListAsync(cancellationToken);

        return documents.Select(CityDocumentMapper.ToEntity).ToList();
    }

    public async Task UpdateAsync(City city, CancellationToken cancellationToken = default)
    {
        var document = CityDocumentMapper.ToDocument(city);
        await _context.Cities.ReplaceOneAsync(
            existing => existing.Id == city.Id,
            document,
            cancellationToken: cancellationToken);
    }

    private static FilterDefinition<CityDocument> BuildStatusFilter(CityStatusFilter status) =>
        status switch
        {
            CityStatusFilter.Active => Builders<CityDocument>.Filter.Eq(city => city.IsActive, true),
            CityStatusFilter.Inactive => Builders<CityDocument>.Filter.Eq(city => city.IsActive, false),
            _ => Builders<CityDocument>.Filter.Empty,
        };
}
