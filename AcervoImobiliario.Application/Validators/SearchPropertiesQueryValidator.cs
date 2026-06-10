using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Properties;

namespace AcervoImobiliario.Application.Validators;

public static class SearchPropertiesQueryValidator
{
    public static Result Validate(SearchPropertiesQuery query)
    {
        var hasCityId = HasValue(query.CityId);
        var hasNeighborhood = HasValue(query.Neighborhood);
        var hasStreet = HasValue(query.Street);
        var hasNumber = HasValue(query.Number);
        var hasComplement = HasValue(query.Complement);
        var hasCadastralIndex = HasValue(query.CadastralIndex);

        if (hasNumber && !hasCityId && !hasNeighborhood && !hasStreet && !hasCadastralIndex)
        {
            return Result.ValidationFailure("Não é permitido buscar somente por number.");
        }

        if (hasComplement && !hasCityId && !hasNeighborhood && !hasStreet && !hasNumber && !hasCadastralIndex)
        {
            return Result.ValidationFailure("Não é permitido buscar somente por complement.");
        }

        if (hasNeighborhood && !hasCityId)
        {
            return Result.ValidationFailure("Não é permitido buscar por neighborhood sem cityId.");
        }

        if (hasStreet && (!hasCityId || !hasNeighborhood))
        {
            return Result.ValidationFailure("Não é permitido buscar por street sem cityId e neighborhood.");
        }

        if (hasNumber && (!hasCityId || !hasNeighborhood || !hasStreet))
        {
            return Result.ValidationFailure("Não é permitido buscar por number sem cityId, neighborhood e street.");
        }

        if (hasComplement && (!hasCityId || !hasNeighborhood || !hasStreet || !hasNumber))
        {
            return Result.ValidationFailure(
                "complement exige cityId, neighborhood, street e number.");
        }

        if (!hasCityId && !hasCadastralIndex)
        {
            return Result.ValidationFailure("Informe cityId ou cadastralIndex para realizar a busca.");
        }

        if (hasCadastralIndex && (hasCityId || hasNeighborhood || hasStreet || hasNumber || hasComplement))
        {
            return Result.ValidationFailure(
                "A busca por cadastralIndex não pode ser combinada com outros filtros de endereço.");
        }

        return Result.Success();
    }

    private static bool HasValue(string? value) => !string.IsNullOrWhiteSpace(value);
}
