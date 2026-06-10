using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using AcervoImobiliario.Application.Interfaces;

namespace AcervoImobiliario.Application.Services;

public sealed partial class AddressNormalizationService : IAddressNormalizationService
{
    private static readonly (string Word, string Abbreviation)[] Abbreviations =
    [
        ("APARTAMENTO", "APT"),
        ("APTO", "APT"),
        ("BLOCO", "BLOCO"),
        ("LOJA", "LOJA"),
        ("SALA", "SALA"),
        ("CASA", "CASA"),
        ("LOTE", "LOTE"),
        ("QUADRA", "QUADRA"),
        ("FUNDOS", "FUNDOS"),
        ("COBERTURA", "COBERTURA"),
    ];

    public string NormalizeComplement(string? complement)
    {
        if (string.IsNullOrWhiteSpace(complement))
        {
            return string.Empty;
        }

        var normalized = RemoveAccents(complement.Trim()).ToUpperInvariant();
        normalized = ConsecutiveSeparatorsRegex().Replace(normalized, " ");
        normalized = DuplicateSpacesRegex().Replace(normalized, " ");
        normalized = ApplyAbbreviations(normalized);
        normalized = DuplicateSpacesRegex().Replace(normalized, " ");
        return normalized.Trim();
    }

    private static string ApplyAbbreviations(string value)
    {
        var result = value;

        foreach (var (word, abbreviation) in Abbreviations)
        {
            result = Regex.Replace(
                result,
                $@"\b{Regex.Escape(word)}\.?\b",
                abbreviation,
                RegexOptions.CultureInvariant);
        }

        return result;
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

    [GeneratedRegex(@"[.\-/_,;]+")]
    private static partial Regex ConsecutiveSeparatorsRegex();
}
