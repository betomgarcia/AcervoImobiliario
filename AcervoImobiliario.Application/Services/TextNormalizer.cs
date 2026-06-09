using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using AcervoImobiliario.Application.Interfaces;

namespace AcervoImobiliario.Application.Services;

public sealed partial class TextNormalizer : ITextNormalizer
{
    public string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var withoutAccents = RemoveAccents(value.Trim());
        var lowerCase = withoutAccents.ToLowerInvariant();
        return DuplicateSpacesRegex().Replace(lowerCase, " ");
    }

    private static string RemoveAccents(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex DuplicateSpacesRegex();
}
