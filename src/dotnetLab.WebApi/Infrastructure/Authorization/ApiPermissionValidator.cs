namespace dotnetLab.WebApi.Infrastructure.Authorization;

/// <summary>
/// permission validator
/// </summary>
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