using Microsoft.Extensions.DependencyInjection;

namespace dotnetLab.UseCase;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCoreApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);

        return serviceCollection;
    }
}