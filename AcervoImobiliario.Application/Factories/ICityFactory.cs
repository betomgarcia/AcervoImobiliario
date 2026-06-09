using AcervoImobiliario.Domain.Entities;

namespace AcervoImobiliario.Application.Factories;

public interface ICityFactory
{
    City Create(string id, string name, string state);

    void Update(City city, string name, string state, bool isActive);
}
