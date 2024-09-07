namespace dotnetLab.WebApi.Infrastructure.Authorization;

/// <summary>
/// 權限驗證器
/// </summary>
public interface IApiPermissionValidator
{
    /// <summary>
    /// 驗證使用者名稱在指定的 api 規則下有沒有權限
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="permissionRule"></param>
    /// <returns></returns>
    Task<bool> VerifyAsync(string? userName, string? permissionRule);
}