using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace dotnetLab.WebApi.Infrastructure.OpenApiTransformers.OperationTransformers;

/// <summary>
/// api response status code 資訊補充 - auth error
/// </summary>
public class ApiAuthErrorResponseOperationTransformer : IOpenApiOperationTransformer
{
    /// <summary>Transforms the specified OpenAPI operation.</summary>
    /// <param name="operation">The <see cref="T:Microsoft.OpenApi.Models.OpenApiOperation" /> to modify.</param>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.OpenApi.OpenApiOperationTransformerContext" /> associated with the <see paramref="operation" />.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        if (!context.Description.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>().Any())
        {
            return Task.CompletedTask;
        }

        operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

        return Task.CompletedTask;
    }
}