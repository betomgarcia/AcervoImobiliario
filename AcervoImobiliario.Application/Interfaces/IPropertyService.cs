using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;

namespace AcervoImobiliario.Application.Interfaces;

public interface IPropertyService
{
    Task<Result<IReadOnlyList<PropertyResponse>>> SearchAsync(
        SearchPropertiesQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<PropertyResponse>> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<Result<PropertyResponse>> CreateAsync(
        CreatePropertyRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<PropertyResponse>> UpdateAsync(
        string id,
        UpdatePropertyRequest request,
        CancellationToken cancellationToken = default);
}
