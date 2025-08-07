using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace dotnetLab.WebApi.Infrastructure.OpenApiTransformers.OperationTransformers;

/// <summary>
/// api 上的身份認證設定轉換器
/// </summary>
public class ApiSecurityOperationTransformer : IOpenApiOperationTransformer
{
    /// <summary>Transforms the specified OpenAPI operation.</summary>
    /// <param name="operation">The <see cref="T:Microsoft.OpenApi.Models.OpenApiOperation" /> to modify.</param>
    /// <param name="context">
    /// The <see cref="T:Microsoft.AspNetCore.OpenApi.OpenApiOperationTransformerContext" /> associated with the
    /// <see paramref="operation" />.
    /// </param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        if (!context.Description.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>().Any())
        {
            return Task.CompletedTask;
        }

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "OAuth2"
                    }
                }
            ] = new List<string>
            {
            }
            // ] = new List<string> { authOptions.Audience }
        });

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }
            ] = new List<string>
            {
            }
            // ] = new List<string> { authOptions.Audience }
        });

        return Task.CompletedTask;
    }
}