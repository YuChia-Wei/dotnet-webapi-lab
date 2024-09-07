namespace dotnetLab.WebApi.Infrastructure.Authorization;

/// <summary>
/// permission validator Fake implement
/// </summary>
/// <remarks>
/// 你可以自己實做一個獨立的 api 權限驗證器
/// </remarks>
public class ApiPermissionValidator : IApiPermissionValidator
{
    /// <summary>
    /// verify user
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="permissionRule"></param>
    /// <returns></returns>
    public Task<bool> VerifyAsync(string? userName, string? permissionRule)
    {
        return Task.FromResult(true);
    }
}