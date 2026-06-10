using AcervoImobiliario.Application.Factories;
using AcervoImobiliario.Application.Interfaces;
using AcervoImobiliario.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AcervoImobiliario.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<ITextNormalizer, TextNormalizer>();
        services.AddSingleton<IAddressNormalizationService, AddressNormalizationService>();
        services.AddScoped<ICityFactory, CityFactory>();
        services.AddScoped<IPropertyFactory, PropertyFactory>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<IPropertyAutocompleteService, PropertyAutocompleteService>();
        services.AddScoped<IPropertyHistoryService, PropertyHistoryService>();

        return services;
    }
}
