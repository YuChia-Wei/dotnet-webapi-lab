using dotnetLab.Application.Inventories.UseCases;
using dotnetLab.Application.Orders.UseCases;
using dotnetLab.Application.Products.UseCases;
using dotnetLab.Application.Shipments.UseCases;
using dotnetLab.Application.SimpleDocument.UseCases;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;

namespace dotnetLab.Application;

/// <summary>註冊應用程式服務與 Wolverine 訊息轉接器。</summary>
public static class ServiceCollectionExtension
{
    /// <summary>註冊 Use Case 與目前組件的訊息處理器。</summary>
    public static IServiceCollection AddCoreApplication(this IServiceCollection serviceCollection)
    {
        // 註冊 Wolverine
        serviceCollection.AddWolverine(options =>
        {
            // 指定組件位置
            // 因為目前類別就位於 use case，因此指定自己，避免因業務邏輯的變更而影響元件掃描
            options.Discovery.IncludeAssembly(typeof(ServiceCollectionExtension).Assembly);
        });
        // 另外的註冊方式是要使用 builder host 去註冊，讓系統去掃描所有有使用到 IMessageBus 介面，或有符合 Handler 實作命名的類別並自動註冊
        // builder.Host.UseWolverine();

        serviceCollection.AddScoped<IGetInventoryUseCase, GetInventoryUseCase>();
        serviceCollection.AddScoped<IGetOrderUseCase, GetOrderUseCase>();
        serviceCollection.AddScoped<IGetProductUseCase, GetProductUseCase>();
        serviceCollection.AddScoped<IGetShipmentUseCase, GetShipmentUseCase>();
        serviceCollection.AddScoped<IGetSimpleDocumentUseCase, GetSimpleDocumentUseCase>();
        serviceCollection.AddScoped<ICreateSimpleDocumentUseCase, CreateSimpleDocumentUseCase>();
        serviceCollection.AddScoped<IUpdateSimpleDocumentDescriptionUseCase, UpdateSimpleDocumentDescriptionUseCase>();

        return serviceCollection;
    }
}