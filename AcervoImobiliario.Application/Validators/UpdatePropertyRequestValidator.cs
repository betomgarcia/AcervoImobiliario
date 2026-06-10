using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;

namespace AcervoImobiliario.Application.Validators;

public static class UpdatePropertyRequestValidator
{
    public static Result Validate(UpdatePropertyRequest request)
    {
        return PropertyAddressValidator.Validate(
            request.CityId,
            request.Neighborhood,
            request.Street,
            request.Number);
    }
}
