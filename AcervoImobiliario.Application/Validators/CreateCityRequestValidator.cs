using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.Cities;

namespace AcervoImobiliario.Application.Validators;

public static class CreateCityRequestValidator
{
  private const int StateLength = 2;

  public static Result Validate(CreateCityRequest request)
  {
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(request.Name))
    {
      errors[nameof(request.Name)] = ["O nome da cidade é obrigatório."];
    }

    if (string.IsNullOrWhiteSpace(request.State))
    {
      errors[nameof(request.State)] = ["O estado da cidade é obrigatório."];
    }
    else if (request.State.Trim().Length != StateLength)
    {
      errors[nameof(request.State)] = ["O estado deve conter exatamente 2 caracteres."];
    }

    if (errors.Count > 0)
    {
      return Result.ValidationFailure(errors);
    }

    return Result.Success();
  }
}
