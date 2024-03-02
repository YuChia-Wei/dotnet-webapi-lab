using Microsoft.AspNetCore.Authorization;

namespace dotnetLab.WebApi.Infrastructure.Authorization.Policy;

/// <summary>
/// Authorization Policy - 需要 LoginUser 資訊
/// </summary>
public class LoginUserRequestedPolicy
{
    /// <summary>
    /// Policy 行為
    /// </summary>
    /// <returns></returns>
    public static Action<AuthorizationPolicyBuilder> PolicyAction()
    {
        return builder => builder.RequireClaim("name");
    }
}