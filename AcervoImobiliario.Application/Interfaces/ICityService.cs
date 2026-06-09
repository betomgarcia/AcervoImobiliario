using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Cities;

namespace AcervoImobiliario.Application.Interfaces;

public interface ICityService
{
    Task<Result<IReadOnlyList<CityResponse>>> ListActiveAsync(CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<CityResponse>>> SearchAsync(
        string term,
        CancellationToken cancellationToken = default);

    Task<Result<CityResponse>> CreateAsync(
        CreateCityRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<CityResponse>> UpdateAsync(
        string id,
        UpdateCityRequest request,
        CancellationToken cancellationToken = default);
}
