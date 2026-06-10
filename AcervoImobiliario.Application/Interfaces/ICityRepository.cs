using AcervoImobiliario.Application.DTOs.Cities;
using AcervoImobiliario.Domain.Entities;

namespace AcervoImobiliario.Application.Interfaces;

public interface ICityRepository
{
    Task<City?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<City?> GetByNameNormalizedAndStateAsync(
        string nameNormalized,
        string state,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<City>> ListAsync(
        string? nameNormalized,
        CityStatusFilter status,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<City>> ListActiveAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<City>> SearchActiveByNameNormalizedAsync(
        string termNormalized,
        CancellationToken cancellationToken = default);

    Task CreateAsync(City city, CancellationToken cancellationToken = default);

    Task UpdateAsync(City city, CancellationToken cancellationToken = default);
}
