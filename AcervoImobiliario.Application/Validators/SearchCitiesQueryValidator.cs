using AcervoImobiliario.Application.Common;

namespace AcervoImobiliario.Application.Validators;

public static class SearchCitiesQueryValidator
{
  private const int MinimumTermLength = 2;

  public static Result Validate(string? term)
  {
    if (string.IsNullOrWhiteSpace(term) || term.Trim().Length < MinimumTermLength)
    {
      return Result.ValidationFailure(
          $"O parâmetro term deve conter no mínimo {MinimumTermLength} caracteres.");
    }

    return Result.Success();
  }
}
