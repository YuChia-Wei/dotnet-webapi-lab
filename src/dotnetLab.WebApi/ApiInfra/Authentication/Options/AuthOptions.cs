namespace dotnetLab.WebApi.Infrastructure.Authentication.Options;

/// <summary>
/// OAuth 設定參數
/// </summary>
public class AuthOptions
{
    /// <summary>
    /// Config 常數
    /// </summary>
    public const string Auth = "Auth";

    /// <summary>
    /// Authority Url
    /// </summary>
    /// <remarks>
    /// Authority：用來指向身份驗證伺服器的基礎 URL。
    /// Issuer：發行 token 的實體，通常在 token 的 iss claim 中指定。
    /// </remarks>
    public string Authority { get; set; }

    // <summary>
    // auth server issuer
    // </summary>
    // public string Issuer { get; set; }

    /// <summary>
    /// authorization endpoint in oauth 2 (opid) server
    /// </summary>
    public string AuthorizationEndpoint { get; set; }

    /// <summary>
    /// token endpoint in oauth 2 (opid) server
    /// </summary>
    public string TokenEndpoint { get; set; }

    /// <summary>
    /// ClientId
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    /// <value>The client secret.</value>
    public string ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the audience.
    /// </summary>
    /// <value>The audience.</value>
    public string Audience { get; set; }
}