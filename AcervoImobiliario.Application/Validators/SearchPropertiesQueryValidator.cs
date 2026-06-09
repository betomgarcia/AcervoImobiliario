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
        var hasComplementType = query.ComplementType.HasValue;
        var hasComplementValue = HasValue(query.ComplementValue);
        var hasCadastralIndex = HasValue(query.CadastralIndex);

        if (hasNumber && !hasCityId && !hasNeighborhood && !hasStreet && !hasCadastralIndex)
        {
            return Result.ValidationFailure("Não é permitido buscar somente por number.");
        }

        if (hasComplementType && !hasCityId && !hasNeighborhood && !hasStreet && !hasNumber && !hasCadastralIndex)
        {
            return Result.ValidationFailure("Não é permitido buscar somente por complementType.");
        }

        if (hasComplementValue && !hasComplementType)
        {
            return Result.ValidationFailure("Não é permitido buscar somente por complementValue.");
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

        if ((hasComplementType || hasComplementValue)
            && (!hasCityId || !hasNeighborhood || !hasStreet || !hasNumber))
        {
            return Result.ValidationFailure(
                "complementType e complementValue exigem cityId, neighborhood, street e number.");
        }

        if (hasComplementType && !hasComplementValue)
        {
            return Result.ValidationFailure(
                "complementValue é obrigatório quando complementType for informado na busca.");
        }

        if (!hasCityId && !hasCadastralIndex)
        {
            return Result.ValidationFailure("Informe cityId ou cadastralIndex para realizar a busca.");
        }

        if (hasCadastralIndex && (hasCityId || hasNeighborhood || hasStreet || hasNumber || hasComplementType || hasComplementValue))
        {
            return Result.ValidationFailure(
                "A busca por cadastralIndex não pode ser combinada com outros filtros de endereço.");
        }

        return Result.Success();
    }

    private static bool HasValue(string? value) => !string.IsNullOrWhiteSpace(value);
}
