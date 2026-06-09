using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Infrastructure.Configuration;
using AcervoImobiliario.Infrastructure.Persistence;
using AcervoImobiliario.Infrastructure.Repositories;
using AcervoImobiliario.Infrastructure.Seeding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AcervoImobiliario.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(MongoDbSettings.SectionName));

        services.AddSingleton<MongoDbContext>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IPropertyHistoryRepository, PropertyHistoryRepository>();
        services.AddScoped<MongoDbIndexInitializer>();
        services.AddScoped<CitySeedInitializer>();
        services.AddScoped<MongoDbInitializer>();

        return services;
    }
}
