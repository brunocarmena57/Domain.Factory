using System.Diagnostics.CodeAnalysis;
using Bruno57.Domain.Factory.CacheService;
using Bruno57.Domain.Factory.Handlers;
using Bruno57.Domain.Factory.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bruno57.Domain.Factory.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class DomainFactoryExtensions
{
    public static IServiceCollection AddDomainFactory(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped(typeof(IDomainFactory<,>), typeof(CreateEntityObjectFactory<,>));
        serviceCollection.AddScoped<IFactoryMethodHandler, FactoryMethodHandler>();
        serviceCollection.AddScoped<ICacheProvider, CacheProvider>();
        serviceCollection.AddScoped<IReadCache, CacheProvider>();
        serviceCollection.AddMemoryCache(options =>
        {
            options.ExpirationScanFrequency = TimeSpan.FromHours(1);
        });
            
        return serviceCollection;
    }
}
