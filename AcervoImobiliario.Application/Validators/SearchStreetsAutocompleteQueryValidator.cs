using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;

namespace AcervoImobiliario.Application.Validators;

public static class SearchStreetsAutocompleteQueryValidator
{
    private const int MinimumTermLength = 2;

    public static Result Validate(SearchStreetsAutocompleteQuery query)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(query.CityId))
        {
            errors[nameof(query.CityId)] = ["O parâmetro cityId é obrigatório."];
        }

        if (string.IsNullOrWhiteSpace(query.Neighborhood))
        {
            errors[nameof(query.Neighborhood)] = ["O parâmetro neighborhood é obrigatório."];
        }

        if (string.IsNullOrWhiteSpace(query.Term) || query.Term.Trim().Length < MinimumTermLength)
        {
            errors[nameof(query.Term)] =
                [$"O parâmetro term deve conter no mínimo {MinimumTermLength} caracteres."];
        }

        if (errors.Count > 0)
        {
            return Result.ValidationFailure(errors);
        }

        return Result.Success();
    }
}
