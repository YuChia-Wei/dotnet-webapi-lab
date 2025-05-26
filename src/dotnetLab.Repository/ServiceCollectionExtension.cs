using dotnetLab.Database.SampleDb;
using dotnetLab.Repository.Factories;
using dotnetLab.Repository.Implements;
using dotnetLab.UseCases.SimpleDocument.Ports.Out;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dotnetLab.Repository;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDataSource(this IServiceCollection serviceCollection)
    {
        // serviceCollection.AddEfCore();

        // serviceCollection.AddDbConnectionFactory();

        serviceCollection.AddSimpleDocumentRepository();

        return serviceCollection;
    }

    private static IServiceCollection AddDbConnectionFactory(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        return serviceCollection;
    }

    private static IServiceCollection AddEfCore(this IServiceCollection serviceCollection)
    {
        // 註冊 EF Core Db Context
        serviceCollection.AddDbContext<SampleDbContext>(
            (provider, builder) =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

                builder.UseLoggerFactory(loggerFactory)
                       // .UseSqlServer("connection-string")
                       .UseNpgsql("connection-string")
                       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            },
            ServiceLifetime.Scoped,
            ServiceLifetime.Singleton);

        return serviceCollection;
    }

    private static IServiceCollection AddSimpleDocumentRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ISimpleDocumentRepository, MockDataRepository>();

        // serviceCollection.AddScoped<ISimpleDocumentRepository, SimpleDocumentRepository>();

        // serviceCollection.AddScoped<ISimpleDocumentRepository, GrpcSampleDataRepository>();

        return serviceCollection;
    }
}