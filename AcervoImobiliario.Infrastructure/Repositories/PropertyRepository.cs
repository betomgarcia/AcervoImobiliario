using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Domain.Entities;
using AcervoImobiliario.Domain.Enums;
using AcervoImobiliario.Infrastructure.Persistence;
using AcervoImobiliario.Infrastructure.Persistence.Documents;
using AcervoImobiliario.Infrastructure.Persistence.Mappers;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace AcervoImobiliario.Infrastructure.Repositories;

public sealed class PropertyRepository : IPropertyRepository
{
    private readonly MongoDbContext _context;

    public PropertyRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Property property, CancellationToken cancellationToken = default)
    {
        var document = PropertyDocumentMapper.ToDocument(property);
        await _context.Properties.InsertOneAsync(document, cancellationToken: cancellationToken);
    }

    public async Task<Property?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var document = await _context.Properties
            .Find(property => property.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return document is null ? null : PropertyDocumentMapper.ToEntity(document);
    }

    public async Task<Property?> GetByUniqueAddressAsync(
        string cityId,
        string neighborhoodNormalized,
        string streetNormalized,
        string number,
        ComplementType complementType,
        string? complementValueNormalized,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<PropertyDocument>.Filter.And(
            Builders<PropertyDocument>.Filter.Eq(property => property.CityId, cityId),
            Builders<PropertyDocument>.Filter.Eq(property => property.NeighborhoodNormalized, neighborhoodNormalized),
            Builders<PropertyDocument>.Filter.Eq(property => property.StreetNormalized, streetNormalized),
            Builders<PropertyDocument>.Filter.Eq(property => property.Number, number),
            Builders<PropertyDocument>.Filter.Eq(property => property.ComplementType, complementType),
            complementValueNormalized is null
                ? Builders<PropertyDocument>.Filter.Eq(property => property.ComplementValueNormalized, null)
                : Builders<PropertyDocument>.Filter.Eq(
                    property => property.ComplementValueNormalized,
                    complementValueNormalized));

        var document = await _context.Properties
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        return document is null ? null : PropertyDocumentMapper.ToEntity(document);
    }

    public async Task<IReadOnlyList<Property>> SearchAsync(
        PropertySearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var filters = new List<FilterDefinition<PropertyDocument>>();

        if (criteria.ActiveOnly)
        {
            filters.Add(Builders<PropertyDocument>.Filter.Eq(property => property.IsActive, true));
        }

        if (!string.IsNullOrWhiteSpace(criteria.CadastralIndex))
        {
            filters.Add(Builders<PropertyDocument>.Filter.Eq(
                property => property.CadastralIndex,
                criteria.CadastralIndex));
        }
        else
        {
            filters.Add(Builders<PropertyDocument>.Filter.Eq(property => property.CityId, criteria.CityId));

            if (!string.IsNullOrWhiteSpace(criteria.NeighborhoodNormalized))
            {
                filters.Add(Builders<PropertyDocument>.Filter.Eq(
                    property => property.NeighborhoodNormalized,
                    criteria.NeighborhoodNormalized));
            }

            if (!string.IsNullOrWhiteSpace(criteria.StreetNormalized))
            {
                filters.Add(Builders<PropertyDocument>.Filter.Eq(
                    property => property.StreetNormalized,
                    criteria.StreetNormalized));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Number))
            {
                filters.Add(Builders<PropertyDocument>.Filter.Eq(
                    property => property.Number,
                    criteria.Number));
            }

            if (criteria.ComplementType.HasValue)
            {
                filters.Add(Builders<PropertyDocument>.Filter.Eq(
                    property => property.ComplementType,
                    criteria.ComplementType.Value));

                filters.Add(criteria.ComplementValueNormalized is null
                    ? Builders<PropertyDocument>.Filter.Eq(property => property.ComplementValueNormalized, null)
                    : Builders<PropertyDocument>.Filter.Eq(
                        property => property.ComplementValueNormalized,
                        criteria.ComplementValueNormalized));
            }
        }

        var filter = filters.Count == 1
            ? filters[0]
            : Builders<PropertyDocument>.Filter.And(filters);

        var documents = await _context.Properties
            .Find(filter)
            .SortBy(property => property.NeighborhoodNormalized)
            .ThenBy(property => property.StreetNormalized)
            .ThenBy(property => property.Number)
            .ToListAsync(cancellationToken);

        return documents.Select(PropertyDocumentMapper.ToEntity).ToList();
    }

    public async Task<IReadOnlyList<string>> SearchDistinctNeighborhoodsAsync(
        string cityId,
        string termNormalized,
        CancellationToken cancellationToken = default)
    {
        var filter = BuildActiveAutocompleteFilter(cityId)
            & Builders<PropertyDocument>.Filter.Regex(
                property => property.NeighborhoodNormalized,
                BuildContainsRegex(termNormalized));

        return await GetDistinctSortedValuesAsync(
            filter,
            document => document.Neighborhood,
            alphabetical: true,
            cancellationToken);
    }

    public async Task<IReadOnlyList<string>> SearchDistinctStreetsAsync(
        string cityId,
        string neighborhoodNormalized,
        string termNormalized,
        CancellationToken cancellationToken = default)
    {
        var filter = BuildActiveAutocompleteFilter(cityId)
            & Builders<PropertyDocument>.Filter.Eq(
                property => property.NeighborhoodNormalized,
                neighborhoodNormalized)
            & Builders<PropertyDocument>.Filter.Regex(
                property => property.StreetNormalized,
                BuildContainsRegex(termNormalized));

        return await GetDistinctSortedValuesAsync(
            filter,
            document => document.Street,
            alphabetical: true,
            cancellationToken);
    }

    public async Task<IReadOnlyList<string>> SearchDistinctNumbersAsync(
        string cityId,
        string neighborhoodNormalized,
        string streetNormalized,
        string term,
        CancellationToken cancellationToken = default)
    {
        var filter = BuildActiveAutocompleteFilter(cityId)
            & Builders<PropertyDocument>.Filter.Eq(
                property => property.NeighborhoodNormalized,
                neighborhoodNormalized)
            & Builders<PropertyDocument>.Filter.Eq(
                property => property.StreetNormalized,
                streetNormalized)
            & Builders<PropertyDocument>.Filter.Regex(
                property => property.Number,
                BuildContainsRegex(term));

        return await GetDistinctSortedValuesAsync(
            filter,
            document => document.Number,
            alphabetical: false,
            cancellationToken);
    }

    private static FilterDefinition<PropertyDocument> BuildActiveAutocompleteFilter(string cityId) =>
        Builders<PropertyDocument>.Filter.And(
            Builders<PropertyDocument>.Filter.Eq(property => property.CityId, cityId),
            Builders<PropertyDocument>.Filter.Eq(property => property.IsActive, true));

    private static BsonRegularExpression BuildContainsRegex(string term) =>
        new(Regex.Escape(term), "i");

    private async Task<IReadOnlyList<string>> GetDistinctSortedValuesAsync(
        FilterDefinition<PropertyDocument> filter,
        Func<PropertyDocument, string> valueSelector,
        bool alphabetical,
        CancellationToken cancellationToken)
    {
        var documents = await _context.Properties
            .Find(filter)
            .ToListAsync(cancellationToken);

        var distinctValues = documents
            .Select(valueSelector)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return alphabetical
            ? distinctValues.OrderBy(value => value, StringComparer.OrdinalIgnoreCase).ToList()
            : distinctValues
                .OrderBy(value => int.TryParse(value, out var number) ? number : int.MaxValue)
                .ThenBy(value => value, StringComparer.Ordinal)
                .ToList();
    }

    public async Task<IReadOnlyList<Property>> ListAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _context.Properties
            .Find(FilterDefinition<PropertyDocument>.Empty)
            .SortByDescending(property => property.CreatedAt)
            .ToListAsync(cancellationToken);

        return documents.Select(PropertyDocumentMapper.ToEntity).ToList();
    }

    public async Task UpdateAsync(Property property, CancellationToken cancellationToken = default)
    {
        var document = PropertyDocumentMapper.ToDocument(property);
        await _context.Properties.ReplaceOneAsync(
            existing => existing.Id == property.Id,
            document,
            cancellationToken: cancellationToken);
    }
}
