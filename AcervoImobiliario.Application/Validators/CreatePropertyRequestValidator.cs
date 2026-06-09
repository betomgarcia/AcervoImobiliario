using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;

namespace AcervoImobiliario.Application.Validators;

public static class CreatePropertyRequestValidator
{
    public static Result Validate(CreatePropertyRequest request)
    {
        return PropertyAddressValidator.Validate(
            request.CityId,
            request.Neighborhood,
            request.Street,
            request.Number,
            request.ComplementType,
            request.ComplementValue);
    }
}
