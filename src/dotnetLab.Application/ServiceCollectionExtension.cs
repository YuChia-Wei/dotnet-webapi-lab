using Microsoft.Extensions.DependencyInjection;

namespace dotnetLab.Application;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCoreApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);

        return serviceCollection;
    }
}