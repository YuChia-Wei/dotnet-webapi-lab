using dotnetLab.UseCase.SimpleDocument.Queries;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;

namespace dotnetLab.UseCase;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCoreApplication(this IServiceCollection serviceCollection)
    {
        // 註冊 Wolverine
        serviceCollection.AddWolverine(options =>
        {
            // 指定組件位置
            options.Discovery.IncludeAssembly(typeof(SimpleDocQuery).Assembly);
        });
        // 另外的註冊方式是要使用 builder host 去註冊，讓系統去掃描所有有使用到 IMessageBus 介面，或有符合 Handler 實作命名的類別並自動註冊
        // builder.Host.UseWolverine();

        return serviceCollection;
    }
}