using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.Enums;

namespace AcervoImobiliario.Application.Validators;

public static class ListPropertyHistoriesQueryValidator
{
    public static Result<HistorySortDirection> ParseSortDirection(string? sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortDirection))
        {
            return Result<HistorySortDirection>.Success(HistorySortDirection.Desc);
        }

        if (!Enum.TryParse<HistorySortDirection>(sortDirection, ignoreCase: true, out var parsed))
        {
            return Result<HistorySortDirection>.ValidationFailure(
                "O parâmetro sortDirection deve ser 'asc' ou 'desc'.");
        }

        return Result<HistorySortDirection>.Success(parsed);
    }
}
