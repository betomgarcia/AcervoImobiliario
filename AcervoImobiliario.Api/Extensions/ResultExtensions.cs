using AcervoImobiliario.Api.Models;
using AcervoImobiliario.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace AcervoImobiliario.Api.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToActionResult(this Result result, Func<ActionResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess();
        }

        return result.ToErrorActionResult();
    }

    public static ActionResult<T> ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value!;
        }

        return result.ToErrorActionResult();
    }

    public static ActionResult<T> ToCreatedResult<T>(
        this Result<T> result,
        Func<T, string> locationFactory)
    {
        if (result.IsSuccess)
        {
            return new CreatedResult(locationFactory(result.Value!), result.Value);
        }

        return result.ToErrorActionResult();
    }

    private static ObjectResult ToErrorActionResult(this Result result)
    {
        var error = result.Error!;
        var response = new ApiErrorResponse
        {
            Success = false,
            Message = error.Message,
            Errors = error.Errors
        };

        var statusCode = error.Kind switch
        {
            ErrorKind.Validation => StatusCodes.Status400BadRequest,
            ErrorKind.NotFound => StatusCodes.Status404NotFound,
            ErrorKind.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return new ObjectResult(response) { StatusCode = statusCode };
    }
}
