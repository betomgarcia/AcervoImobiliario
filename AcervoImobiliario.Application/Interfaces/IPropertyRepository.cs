using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Domain.Entities;
using AcervoImobiliario.Domain.Enums;

namespace AcervoImobiliario.Application.Interfaces;

public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<Property?> GetByUniqueAddressAsync(
        string cityId,
        string neighborhoodNormalized,
        string streetNormalized,
        string number,
        ComplementType complementType,
        string? complementValueNormalized,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Property>> ListAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Property>> SearchAsync(
        PropertySearchCriteria criteria,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> SearchDistinctNeighborhoodsAsync(
        string cityId,
        string termNormalized,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> SearchDistinctStreetsAsync(
        string cityId,
        string neighborhoodNormalized,
        string termNormalized,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> SearchDistinctNumbersAsync(
        string cityId,
        string neighborhoodNormalized,
        string streetNormalized,
        string term,
        CancellationToken cancellationToken = default);

    Task CreateAsync(Property property, CancellationToken cancellationToken = default);

    Task UpdateAsync(Property property, CancellationToken cancellationToken = default);
}
