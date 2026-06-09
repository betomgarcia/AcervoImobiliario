namespace AcervoImobiliario.Application.Common;

public class Result
{
    protected Result(bool isSuccess, ResultError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public ResultError? Error { get; }

    public static Result Success() => new(true, null);

    public static Result Failure(ResultError error) => new(false, error);

    public static Result ValidationFailure(string message, IReadOnlyList<string>? errors = null) =>
        Failure(new ResultError(ErrorKind.Validation, message, errors));

    public static Result ValidationFailure(IReadOnlyDictionary<string, string[]> fieldErrors) =>
        ValidationFailure(
            "Uma ou mais validações falharam.",
            fieldErrors.SelectMany(error => error.Value).ToArray());

    public static Result NotFound(string message) =>
        Failure(new ResultError(ErrorKind.NotFound, message, [message]));

    public static Result Conflict(string message) =>
        Failure(new ResultError(ErrorKind.Conflict, message, [message]));

    public static Result Internal(string message) =>
        Failure(new ResultError(ErrorKind.Internal, message, [message]));
}

public sealed class Result<T> : Result
{
    private Result(bool isSuccess, T? value, ResultError? error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value) => new(true, value, null);

    public static new Result<T> Failure(ResultError error) => new(false, default, error);

    public static new Result<T> ValidationFailure(string message, IReadOnlyList<string>? errors = null) =>
        Failure(new ResultError(ErrorKind.Validation, message, errors));

    public static new Result<T> ValidationFailure(IReadOnlyDictionary<string, string[]> fieldErrors) =>
        ValidationFailure(
            "Uma ou mais validações falharam.",
            fieldErrors.SelectMany(error => error.Value).ToArray());

    public static new Result<T> NotFound(string message) =>
        Failure(new ResultError(ErrorKind.NotFound, message, [message]));

    public static new Result<T> Conflict(string message) =>
        Failure(new ResultError(ErrorKind.Conflict, message, [message]));

    public static Result<T> From(Result result) =>
        Failure(result.Error!);
}
