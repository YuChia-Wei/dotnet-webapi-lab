using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace dotnetLab.WebApi.ApiInfra.OpenApiTransformers.OperationTransformers;

/// <summary>
/// api 上的身份認證設定轉換器
/// </summary>
/// <remarks>
/// 已修正為 .NET 10 搭配的 OpenAPI.NET 2.x（Microsoft.OpenApi 2.x）的版本
/// </remarks>
public class ApiSecurityOperationTransformer : IOpenApiOperationTransformer
{
    /// <summary>Transforms the specified OpenAPI operation.</summary>
    /// <param name="operation">The <see cref="T:OpenApiOperation" /> to modify.</param>
    /// <param name="context">
    /// The <see cref="T:Microsoft.AspNetCore.OpenApi.OpenApiOperationTransformerContext" /> associated with the
    /// <see paramref="operation" />.
    /// </param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        // 建議用 IAuthorizeData，比只抓 AuthorizeAttribute 更完整（包含 Minimal API RequireAuthorization）
        var requiresAuth = context.Description.ActionDescriptor.EndpointMetadata
                                  .OfType<IAuthorizeData>()
                                  .Any();

        if (!requiresAuth)
        {
            return Task.CompletedTask;
        }

        operation.Security ??= new List<OpenApiSecurityRequirement>();

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            // OAuth2：Id 必須和你在 DocumentTransformer 加到 components.securitySchemes 的 key 完全一致（含大小寫）
            [new OpenApiSecuritySchemeReference("OAuth2", context.Document)] = []
        });

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            // Bearer：同上，key 必須一致
            [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
        });

        return Task.CompletedTask;
    }
}