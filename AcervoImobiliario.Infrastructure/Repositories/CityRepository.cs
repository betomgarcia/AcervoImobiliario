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

    public async Task<IReadOnlyList<City>> ListActiveAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _context.Cities
            .Find(city => city.IsActive)
            .SortBy(city => city.NameNormalized)
            .ToListAsync(cancellationToken);

        return documents.Select(CityDocumentMapper.ToEntity).ToList();
    }

    public async Task<IReadOnlyList<City>> SearchByNameNormalizedAsync(
        string termNormalized,
        CancellationToken cancellationToken = default)
    {
        var documents = await _context.Cities
            .Find(city => city.NameNormalized.Contains(termNormalized))
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
}
