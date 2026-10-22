using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace dotnetLab.Tests;

/// <summary>透過真正主機與程序內 HTTP 測試既有路由、回應包裝及 DI。</summary>
public sealed class WebApiBehaviorTests : IClassFixture<WebApiBehaviorTests.ApiFactory>
{
    private readonly HttpClient _client;

    /// <summary>建立不連接外部資料庫或認證服務的測試用戶端。</summary>
    public WebApiBehaviorTests(ApiFactory factory)
    {
        this._client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    /// <summary>四個查詢流程必須透過正確的 Use Case 及預設 adapter 回傳資料。</summary>
    [Theory]
    [InlineData("products", "name", "Mock Product")]
    [InlineData("orders", "customerName", "Mock Customer")]
    [InlineData("shipments", "status", "Created")]
    [InlineData("inventories", "quantity", "10")]
    public async Task Existing_queries_preserve_wrapped_responseAsync(string route, string field, string expected)
    {
        var response = await this._client.GetAsync($"/api/v1/{route}/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(string.IsNullOrWhiteSpace(json.GetProperty("id").GetString()));
        Assert.Equal(expected, json.GetProperty("data").GetProperty(field).ToString());
    }

    /// <summary>零值商品識別碼回傳 404，而非空值錯誤。</summary>
    [Fact]
    public async Task Missing_product_returns_404Async()
    {
        var response = await this._client.GetAsync($"/api/v1/products/{Guid.Empty}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>文件新增回傳整數序號；PATCH 必須回傳布林值。</summary>
    [Fact]
    public async Task Document_create_and_patch_return_their_declared_typesAsync()
    {
        var created = await this._client.PostAsJsonAsync("/api/v1/sample", new { documentNum = "DOC-1", description = "first" });
        Assert.Equal(HttpStatusCode.OK, created.StatusCode);
        var createdJson = await created.Content.ReadFromJsonAsync<JsonElement>();
        var serialId = createdJson.GetProperty("data").GetInt32();
        Assert.Equal(123, serialId);
        var updated = await this._client.PatchAsJsonAsync("/api/v1/sample", new { serialId, description = "changed" });
        Assert.Equal(HttpStatusCode.OK, updated.StatusCode);
        var json = await updated.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.True, json.GetProperty("data").ValueKind);
    }

    /// <summary>無效文件輸入維持 400 驗證結果。</summary>
    [Fact]
    public async Task Invalid_document_input_returns_400Async()
    {
        var response = await this._client.PostAsJsonAsync("/api/v1/sample", new { documentNum = "", description = "test" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>MVC 首頁轉址改用明確路由，不改變瀏覽器的導向結果。</summary>
    [Fact]
    public async Task Root_redirects_to_swaggerAsync()
    {
        var response = await this._client.GetAsync("/");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/swagger", response.Headers.Location?.OriginalString);
    }

    /// <summary>原有一般及業務例外仍回傳 500 JSON。</summary>
    [Theory]
    [InlineData("dotnet-exception")]
    [InlineData("custom-exception")]
    public async Task Exceptions_return_500_with_wrapped_errorAsync(string kind)
    {
        var response = await this._client.GetAsync($"/api/v1/sample/exception/{kind}");
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.TryGetProperty("data", out _));
    }

    /// <summary>保留未登入時的授權拒絕行為，不需連接 Keycloak。</summary>
    [Fact]
    public async Task Protected_endpoint_requires_authenticationAsync()
    {
        var response = await this._client.GetAsync("/api/v1/shouldbeauth");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>OpenAPI 產生仍可使用更新後的安全套件。</summary>
    [Fact]
    public async Task OpenApi_document_can_be_generatedAsync()
    {
        var response = await this._client.GetAsync("/openapi/v1.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.GetProperty("paths").TryGetProperty("/api/v1/products/{productId}", out _));
    }

    /// <summary>在記憶體中啟動真正的 Web API，保留正式 DI 與中介軟體。</summary>
    public sealed class ApiFactory : WebApplicationFactory<Program>
    {
        /// <inheritdoc />
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            builder.UseContentRoot(GetContentRoot());
        }

        private static string GetContentRoot([CallerFilePath] string sourcePath = "") =>
            Path.GetFullPath("../dotnetLab.WebApi", Path.GetDirectoryName(sourcePath)!);
    }
}