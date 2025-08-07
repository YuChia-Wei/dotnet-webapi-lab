using dotnetLab.Persistence.Metadata.SampleDb;
using dotnetLab.Application.Inventories.Ports.Out;
using dotnetLab.Application.Orders.Ports.Out;
using dotnetLab.Application.Products.Ports.Out;
using dotnetLab.Application.Shipments.Ports.Out;
using dotnetLab.Application.SimpleDocument.Ports.Out;
using dotnetLab.Infrastructure.Factories;
using dotnetLab.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dotnetLab.Infrastructure;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDataSource(this IServiceCollection serviceCollection)
    {
        // serviceCollection.AddEfCore();

        // serviceCollection.AddDbConnectionFactory();

        serviceCollection.AddSimpleDocumentRepository();
        serviceCollection.AddOrderRepository();
        serviceCollection.AddShipmentRepository();
        serviceCollection.AddProductRepository();
        serviceCollection.AddInventoryRepository();

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

    private static IServiceCollection AddInventoryRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IInventoryRepository, InventoryMockRepository>();
        return serviceCollection;
    }

    private static IServiceCollection AddOrderRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IOrderRepository, OrderMockRepository>();
        return serviceCollection;
    }

    private static IServiceCollection AddProductRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IProductRepository, ProductMockRepository>();
        return serviceCollection;
    }

    private static IServiceCollection AddShipmentRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IShipmentRepository, ShipmentMockRepository>();
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