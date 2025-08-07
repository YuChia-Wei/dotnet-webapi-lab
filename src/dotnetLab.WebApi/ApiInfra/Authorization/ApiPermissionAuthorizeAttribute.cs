using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dotnetLab.WebApi.Infrastructure.Authorization;

/// <summary>
/// api 權限驗證
/// </summary>
public class ApiPermissionAuthorizeAttribute
    : AuthorizeAttribute,
      // IAuthorizationFilter,
      IAsyncAuthorizationFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeAttribute" /> class.
    /// </summary>
    public ApiPermissionAuthorizeAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Microsoft.AspNetCore.Authorization.AuthorizeAttribute" /> class.
    /// </summary>
    public ApiPermissionAuthorizeAttribute(string permissionRule)
    {
        this.PermissionRule = permissionRule;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Microsoft.AspNetCore.Authorization.AuthorizeAttribute" /> class with the specified policy.
    /// </summary>
    /// <param name="policy">The name of the policy to require for authorization.</param>
    /// <param name="permissionRule"></param>
    public ApiPermissionAuthorizeAttribute(string policy, string permissionRule)
        : base(policy)
    {
        this.PermissionRule = permissionRule;
    }

    /// <summary>
    /// </summary>
    public string? PermissionRule { get; set; }

    /// <summary>
    /// Called early in the filter pipeline to confirm request is authorized.
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext" />.</param>
    /// <returns>
    /// A <see cref="T:System.Threading.Tasks.Task" /> that on completion indicates the filter has executed.
    /// </returns>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            //由於有實作 authorizeAttribute 物件，因此要調用 DI 中的物件時，只能使用 AuthorizationFilterContext 取得
            var apiPermissionValidator = context.HttpContext.RequestServices.GetRequiredService<IApiPermissionValidator>();

            if (!await apiPermissionValidator.VerifyAsync(context.HttpContext.User.Identity?.Name, this.PermissionRule))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}