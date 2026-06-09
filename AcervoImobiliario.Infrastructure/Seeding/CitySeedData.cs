namespace AcervoImobiliario.Infrastructure.Seeding;

internal static class CitySeedData
{
    public static IReadOnlyList<CitySeedEntry> InitialCities { get; } =
    [
        new("Belo Horizonte", "MG"),
        new("Contagem", "MG"),
        new("Betim", "MG")
    ];
}

internal sealed record CitySeedEntry(string Name, string State);
