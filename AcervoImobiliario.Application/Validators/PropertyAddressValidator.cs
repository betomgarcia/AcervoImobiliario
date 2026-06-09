using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Domain.Enums;

namespace AcervoImobiliario.Application.Validators;

public static class PropertyAddressValidator
{
    private static readonly HashSet<ComplementType> ComplementTypesRequiringValue =
    [
        ComplementType.Apartment,
        ComplementType.Room,
        ComplementType.Store
    ];

    public static Result Validate(
        string cityId,
        string neighborhood,
        string street,
        string number,
        ComplementType complementType,
        string? complementValue)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(cityId))
        {
            errors["CityId"] = ["O identificador da cidade é obrigatório."];
        }

        if (string.IsNullOrWhiteSpace(neighborhood))
        {
            errors["Neighborhood"] = ["O bairro é obrigatório."];
        }

        if (string.IsNullOrWhiteSpace(street))
        {
            errors["Street"] = ["A rua é obrigatória."];
        }

        if (string.IsNullOrWhiteSpace(number))
        {
            errors["Number"] = ["O número é obrigatório."];
        }
        else if (!number.Trim().All(char.IsDigit))
        {
            errors["Number"] = ["O número deve conter somente dígitos."];
        }

        if (!Enum.IsDefined(complementType))
        {
            errors["ComplementType"] = ["O tipo de complemento é obrigatório."];
        }
        else if (complementType == ComplementType.None)
        {
            if (!string.IsNullOrWhiteSpace(complementValue))
            {
                errors["ComplementValue"] =
                    ["ComplementValue deve ser vazio ou nulo quando ComplementType for None."];
            }
        }
        else if (ComplementTypesRequiringValue.Contains(complementType)
                 && string.IsNullOrWhiteSpace(complementValue))
        {
            errors["ComplementValue"] =
                [$"ComplementValue é obrigatório quando ComplementType for {complementType}."];
        }

        if (errors.Count > 0)
        {
            return Result.ValidationFailure(errors);
        }

        return Result.Success();
    }
}
