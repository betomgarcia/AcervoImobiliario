namespace AcervoImobiliario.Api.Models;

public sealed class ApiErrorResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public IReadOnlyList<string> Errors { get; init; } = [];
}
