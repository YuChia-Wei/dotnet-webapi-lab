namespace dotnetLab.WebApi.Infrastructure.Authorization;

/// <summary>
/// permission validator Fake implement
/// </summary>
/// <remarks>
/// 你可以自己實做一個獨立的 api 權限驗證器
/// </remarks>
public class ApiPermissionValidator : IApiPermissionValidator
{
    private readonly ILogger<ApiPermissionValidator> _logger;

    /// <summary>
    /// 負責進行 API 權限驗證的實作類別。
    /// </summary>
    public ApiPermissionValidator(ILogger<ApiPermissionValidator> logger)
    {
        this._logger = logger;
    }

    /// <summary>
    /// verify user
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="permissionRule"></param>
    /// <returns></returns>
    public Task<bool> VerifyAsync(string? userName, string? permissionRule)
    {
        this._logger.LogInformation($"User: {userName}, Permission: {permissionRule}");

        return Task.FromResult(true);
    }
}