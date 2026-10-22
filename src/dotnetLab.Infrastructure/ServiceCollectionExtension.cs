using ArchPrecept.Contracts.Application;
using dotnetLab.Application.SimpleDocument.Ports.Out;
using dotnetLab.Domain.Inventories;
using dotnetLab.Domain.Orders;
using dotnetLab.Domain.Products;
using dotnetLab.Domain.SampleDoc;
using dotnetLab.Domain.Shipments;
using dotnetLab.Infrastructure.Factories;
using dotnetLab.Infrastructure.Messaging;
using dotnetLab.Infrastructure.Repositories;
using dotnetLab.Persistence.Metadata.SampleDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dotnetLab.Infrastructure;

/// <summary>註冊外部 adapter 與預設的無資料庫範例實作。</summary>
public static class ServiceCollectionExtension
{
    /// <summary>註冊 scope 內示範儲存庫及領域事件分派器。</summary>
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
        serviceCollection.AddScoped<IAggregateRepository<Inventory, Guid>, InventoryMockRepository>();
        return serviceCollection;
    }

    private static IServiceCollection AddOrderRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAggregateRepository<Order, Guid>, OrderMockRepository>();
        return serviceCollection;
    }

    private static IServiceCollection AddProductRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAggregateRepository<Product, Guid>, ProductMockRepository>();
        return serviceCollection;
    }

    private static IServiceCollection AddShipmentRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAggregateRepository<Shipment, Guid>, ShipmentMockRepository>();
        return serviceCollection;
    }

    private static IServiceCollection AddSimpleDocumentRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAggregateRepository<SimpleDocumentEntity, int>, MockDataRepository>();
        serviceCollection.AddScoped<ISimpleDocumentQueryRepository, SimpleDocumentQueryRepository>();
        serviceCollection.AddScoped<IDomainEventDispatcher, WolverineDomainEventDispatcher>();

        // 替換 SQL adapter 時，將聚合 Port 改註冊為 scoped SimpleDocumentRepository；查詢 adapter 沿用。

        // gRPC adapter 僅供 ISimpleDocumentQueryRepository 使用。

        return serviceCollection;
    }
}