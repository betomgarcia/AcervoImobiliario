namespace AcervoImobiliario.Application.Common;

public sealed class ResultError
{
    public ErrorKind Kind { get; }
    public string Message { get; }
    public IReadOnlyList<string> Errors { get; }

    public ResultError(ErrorKind kind, string message, IReadOnlyList<string>? errors = null)
    {
        Kind = kind;
        Message = message;
        Errors = errors ?? [];
    }
}
