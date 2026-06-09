using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Domain.Entities;

namespace AcervoImobiliario.Application.Factories;

public sealed class CityFactory : ICityFactory
{
    private readonly ITextNormalizer _textNormalizer;

    public CityFactory(ITextNormalizer textNormalizer)
    {
        _textNormalizer = textNormalizer;
    }

    public City Create(string id, string name, string state)
    {
        var nameNormalized = _textNormalizer.Normalize(name);
        return City.Create(id, name, nameNormalized, state);
    }

    public void Update(City city, string name, string state, bool isActive)
    {
        var nameNormalized = _textNormalizer.Normalize(name);
        city.Update(name, nameNormalized, state, isActive);
    }
}
