using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;

namespace AcervoImobiliario.Application.Interfaces;

public interface IPropertyAutocompleteService
{
    Task<Result<IReadOnlyList<string>>> SearchNeighborhoodsAsync(
        SearchNeighborhoodsAutocompleteQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<string>>> SearchStreetsAsync(
        SearchStreetsAutocompleteQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<string>>> SearchNumbersAsync(
        SearchNumbersAutocompleteQuery query,
        CancellationToken cancellationToken = default);
}
