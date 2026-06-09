using AcervoImobiliario.Domain.Exceptions;

namespace AcervoImobiliario.Application.Common;

public static class DomainResult
{
    public static Result<T> Execute<T>(Func<T> action)
    {
        try
        {
            return Result<T>.Success(action());
        }
        catch (DomainException exception)
        {
            return Result<T>.ValidationFailure(exception.Message, [exception.Message]);
        }
    }

    public static async Task<Result<T>> ExecuteAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return Result<T>.Success(await action());
        }
        catch (DomainException exception)
        {
            return Result<T>.ValidationFailure(exception.Message, [exception.Message]);
        }
    }
}
