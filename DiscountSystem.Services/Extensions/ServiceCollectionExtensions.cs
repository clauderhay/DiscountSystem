using DiscountSystem.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

namespace DiscountSystem.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDiscountServices(this IServiceCollection services)
    {
        // Register business services
        services.AddScoped<IDiscountService, DiscountService>();
        services.AddSingleton<ICodeGenerator, CodeGenerator>();
        
        // Add memory cache for performance
        services.AddMemoryCache();
        
        return services;
    }
}