using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.DTOs.PropertyHistories;

namespace AcervoImobiliario.Application.Validators;

public static class CreatePropertyHistoryRequestValidator
{
    public static Result Validate(CreatePropertyHistoryRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (!Enum.IsDefined(request.EventType))
        {
            errors[nameof(request.EventType)] = ["O tipo do evento é obrigatório."];
        }

        if (request.EventDate == default)
        {
            errors[nameof(request.EventDate)] = ["A data do evento é obrigatória."];
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            errors[nameof(request.Description)] = ["A descrição do evento é obrigatória."];
        }

        if (errors.Count > 0)
        {
            return Result.ValidationFailure(errors);
        }

        return Result.Success();
    }
}
