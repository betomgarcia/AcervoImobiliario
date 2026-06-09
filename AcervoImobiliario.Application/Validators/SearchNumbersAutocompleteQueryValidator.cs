using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;

namespace AcervoImobiliario.Application.Validators;

public static class SearchNumbersAutocompleteQueryValidator
{
    private const int MinimumTermLength = 1;

    public static Result Validate(SearchNumbersAutocompleteQuery query)
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

        if (string.IsNullOrWhiteSpace(query.Street))
        {
            errors[nameof(query.Street)] = ["O parâmetro street é obrigatório."];
        }

        if (string.IsNullOrWhiteSpace(query.Term) || query.Term.Trim().Length < MinimumTermLength)
        {
            errors[nameof(query.Term)] =
                [$"O parâmetro term deve conter no mínimo {MinimumTermLength} caractere."];
        }

        if (errors.Count > 0)
        {
            return Result.ValidationFailure(errors);
        }

        return Result.Success();
    }
}
