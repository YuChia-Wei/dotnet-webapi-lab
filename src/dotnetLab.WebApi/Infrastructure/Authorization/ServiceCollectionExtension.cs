namespace dotnetLab.WebApi.Infrastructure.Authorization;

/// <summary>
/// authorization 的 DI 註冊擴充
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    /// 加入 api 權限驗證器
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApiPermissionValidator(this IServiceCollection services)
    {
        services.AddScoped<IApiPermissionValidator, ApiPermissionValidator>();
        return services;
    }
}