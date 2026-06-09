using AcervoImobiliario.Application.Factories;
using AcervoImobiliario.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace AcervoImobiliario.Infrastructure.Seeding;

public sealed class CitySeedInitializer
{
    private readonly ICityRepository _cityRepository;
    private readonly ICityFactory _cityFactory;
    private readonly ITextNormalizer _textNormalizer;
    private readonly ILogger<CitySeedInitializer> _logger;

    public CitySeedInitializer(
        ICityRepository cityRepository,
        ICityFactory cityFactory,
        ITextNormalizer textNormalizer,
        ILogger<CitySeedInitializer> logger)
    {
        _cityRepository = cityRepository;
        _cityFactory = cityFactory;
        _textNormalizer = textNormalizer;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in CitySeedData.InitialCities)
        {
            var nameNormalized = _textNormalizer.Normalize(entry.Name);
            var state = entry.State.Trim().ToUpperInvariant();

            var existingCity = await _cityRepository.GetByNameNormalizedAndStateAsync(
                nameNormalized,
                state,
                cancellationToken);

            if (existingCity is not null)
            {
                _logger.LogDebug(
                    "Seed ignorado: cidade {CityName}/{State} já cadastrada (NameNormalized: {NameNormalized}).",
                    entry.Name,
                    state,
                    nameNormalized);

                continue;
            }

            var city = _cityFactory.Create(Guid.NewGuid().ToString(), entry.Name, state);
            await _cityRepository.CreateAsync(city, cancellationToken);

            _logger.LogInformation(
                "Cidade seed cadastrada: {CityName}/{State} (NameNormalized: {NameNormalized}).",
                entry.Name,
                state,
                nameNormalized);
        }
    }
}
