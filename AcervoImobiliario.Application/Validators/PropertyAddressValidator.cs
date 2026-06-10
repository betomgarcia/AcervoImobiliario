using AcervoImobiliario.Application.Common;

namespace AcervoImobiliario.Application.Validators;

public static class PropertyAddressValidator
{
    public static Result Validate(
        string cityId,
        string neighborhood,
        string street,
        string number)
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

        if (errors.Count > 0)
        {
            return Result.ValidationFailure(errors);
        }

        return Result.Success();
    }
}
