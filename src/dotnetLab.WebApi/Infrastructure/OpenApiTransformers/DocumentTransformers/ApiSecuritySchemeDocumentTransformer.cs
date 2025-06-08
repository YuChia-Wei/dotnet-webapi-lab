using dotnetLab.WebApi.Infrastructure.Authentication.Options;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace dotnetLab.WebApi.Infrastructure.OpenApiTransformers.DocumentTransformers;

/// <summary>
/// api 認證相關設定
/// </summary>
public class ApiSecuritySchemeDocumentTransformer : IOpenApiDocumentTransformer
{
    private readonly AuthOptions _authOptions;

    /// <summary>
    /// 負責轉換 OpenAPI 文件，與 API 安全性相關的設定。
    /// </summary>
    public ApiSecuritySchemeDocumentTransformer(AuthOptions authOptions)
    {
        this._authOptions = authOptions;
    }

    /// <summary>Transforms the specified OpenAPI document.</summary>
    /// <param name="document">The <see cref="T:Microsoft.OpenApi.Models.OpenApiDocument" /> to modify.</param>
    /// <param name="context">
    /// The <see cref="T:Microsoft.AspNetCore.OpenApi.OpenApiDocumentTransformerContext" /> associated with the
    /// <see paramref="document" />.
    /// </param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        // document.Info.Version = "v1";
        // document.Info.Title = $"{AppDomain.CurrentDomain.FriendlyName} V1";
        // document.Info.Description = "";
        var requirements = new Dictionary<string, OpenApiSecurityScheme>
        {
            ["OAuth2"] =
                new OpenApiSecurityScheme
                {
                    Description = @"Authorization Code, 請先勾選 scope: ",
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{this._authOptions.AuthorizationEndpoint}"),
                            TokenUrl = new Uri($"{this._authOptions.TokenEndpoint}"),
                            Scopes = new Dictionary<string, string>
                            {
                                {
                                    this._authOptions.Audience, "Sample Api"
                                }
                            }
                        }
                    }
                },
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // "bearer" refers to the header name here
                In = ParameterLocation.Header,
                BearerFormat = "Json Web Token"
            }
        };
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = requirements;

        return Task.CompletedTask;
    }
}