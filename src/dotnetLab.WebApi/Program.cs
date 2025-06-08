using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning.Conventions;
using dotnetLab.Persistence.Repositories;
using dotnetLab.UseCases;
using dotnetLab.WebApi.Infrastructure.Authentication.Options;
using dotnetLab.WebApi.Infrastructure.Authorization;
using dotnetLab.WebApi.Infrastructure.Authorization.Policy;
using dotnetLab.WebApi.Infrastructure.CustomJsonConverter;
using dotnetLab.WebApi.Infrastructure.OpenApiTransformers.DocumentTransformers;
using dotnetLab.WebApi.Infrastructure.OpenApiTransformers.OperationTransformers;
using dotnetLab.WebApi.Infrastructure.ResponseWrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Formatters;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 處理中文轉碼
builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin,
                                                 UnicodeRanges.CjkUnifiedIdeographs));

// API Url Path 使用小寫
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddControllers(options =>
{
    // 只輸出 Json, 移除輸出 XML
    options.OutputFormatters.RemoveType<XmlDataContractSerializerOutputFormatter>();
    options.Filters.Add<ApiResponseWrappingFilter>();
    options.Filters.Add<ExceptionWrappingFilter>();
}).AddJsonOptions(options =>
{
    // ViewModel 與 Parameter 顯示為小駝峰命名
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

    // 前後端配合，輸出、入統一使用 UTC 時間
    // 設定參考 https://github.com/dotnet/runtime/issues/1566
    options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
});

// **這個 log 暫無法偕同 open-telemetry 或其他對外輸出 log 資料的工具輸出資料**
// 使用 w3c 格式的 log，用來輸出類似 IIS Log 的內容
// 檔案預設存放於 {application path}/logs/ 資料夾下
// 依據本專案所提供的 dockerfile，log 檔案的位置在 container 裡面的 /app/logs 這個位置
builder.Services.AddW3CLogging(logging =>
{
    // Log all W3C fields
    logging.LoggingFields = W3CLoggingFields.All;
    // 5 MB
    logging.FileSizeLimit = 5 * 1024 * 1024;
    logging.RetainedFileCountLimit = 2;
    logging.FileName = AppDomain.CurrentDomain.FriendlyName + "_";
    logging.FlushInterval = TimeSpan.FromSeconds(2);

    //.net 7 new feature
    logging.AdditionalRequestHeaders.Add("x-forwarded-for");
});

// 與 w3c log 一樣，在 .net 6 加入的 http logging 功能，可以記錄所有 http 的傳出入內容並於 console 中輸出
// 配合 et aspnetcore base image 中的 open telemetry ，可以直接將此 log 傳遞給任一 log 集中器
// 這邊使用的範例為預設值，不加也沒關係，有加的話，請注意 LoggingFields 所設定的內容，該處設定值可能會導致一些機敏資料也被記錄下來
// appsettings.json 中請記得在 logging 區塊加上 '"Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware": "Information"' 此設定，避免被忽略掉
// via. https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/?view=aspnetcore-7.0
builder.Services.AddHttpLogging(options =>
{
    // logging fields can via. https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/?view=aspnetcore-7.0#loggingfields
    options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders |
                            HttpLoggingFields.ResponsePropertiesAndHeaders;

    // 加上這個可以將指定的 http header 從 log 中加入/移除
    // options.RequestHeaders.Add("SomeRequestHeader");
    // options.RequestHeaders.Remove("Cookie");
    // options.ResponseHeaders.Add("SomeResponseHeader");
});

//via. https://github.com/dotnet/aspnet-api-versioning
builder.Services.AddApiVersioning(options =>
       {
           options.ReportApiVersions = true;
           options.AssumeDefaultVersionWhenUnspecified = true;
           options.DefaultApiVersion = new ApiVersion(1, 0);
       })
       //加上這個可以使用 namespace 當作版本控制來源
       .AddMvc(o => o.Conventions.Add(new VersionByNamespaceConvention()))
       .AddApiExplorer(options =>
       {
           options.GroupNameFormat = "'v'VVV";
           options.SubstituteApiVersionInUrl = true;
       });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// 在建立 OpenApi json 前先取得設定檔中的 OAuth 設定，以便設定 Auth 資料到產生的 json 中
var authOptions = builder.Configuration
                         .GetSection(AuthOptions.Auth)
                         .Get<AuthOptions>();

// 因為要給 add open api 中的 Transformer 相關物件使用，另外注入
// unsafe......
builder.Services.AddSingleton<AuthOptions>(o => authOptions!);

builder.Services.AddOpenApi(options =>
{
    options.AddOperationTransformer<ApiSecurityOperationTransformer>();
    options.AddOperationTransformer<ApiAuthErrorResponseOperationTransformer>();
    options.AddDocumentTransformer<ApiSecuritySchemeDocumentTransformer>();
});

builder.Services.AddHttpClient();

// 開啟 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyHeader()
                     .AllowAnyMethod();
    });
});

// OAuth
builder.Services
       .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
       {
           options.Authority = authOptions!.Authority;
           options.RequireHttpsMetadata = false;
           options.Audience = authOptions.Audience;
       });

builder.Services.AddAuthorizationBuilder()
       .AddPolicy(nameof(LoginUserRequestedPolicy), LoginUserRequestedPolicy.PolicyAction());

builder.Services.AddApiPermissionValidator();

builder.Services.AddHealthChecks();

builder.Services.AddCoreApplication();

builder.Services.AddDataSource();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//使用自訂物件樣式回應例外訊息
//這邊的例外捕捉是捕捉整個系統的，可以避免其他的系統例外洩露出去
app.UseExceptionHandler(applicationBuilder =>
{
    applicationBuilder.Run(async context =>
    {
        // 取得 ILogger 以便另外撰寫日誌
        var logger = context.RequestServices.GetRequiredService<ILogger>();

        // 取得 IExceptionHandlerPathFeature 的資料，以便後續針對例外內容進行處理
        var exception = context.Features.Get<IExceptionHandlerPathFeature>()!.Error;

        logger.LogError("Exception occured: {ExceptionMessage} , Exception Description: {ExceptionDescription} ",
                        exception.Message,
                        exception.ToString());

        // 建立包含錯誤資訊的 api response 物件
        var failResultViewModel = new ApiResponse<ApiErrorInformation>
        {
            // 取得該次錯誤時的追蹤編號以便設定在 error information 中
            Id = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString(),
            ApiVersion = context.ApiVersioningFeature().RawRequestedApiVersion,
            RequestPath = $"{context.Request.Path}.{context.Request.Method}",
            Data = exception.GetApiErrorInformation()
        };

        await context.Response.WriteAsJsonAsync(failResultViewModel);
    });
});

// 純 Web Api 專案不建議使用此設定
// via https://docs.microsoft.com/zh-tw/aspnet/core/security/enforcing-ssl?view=aspnetcore-6.0&tabs=visual-studio
// app.UseHttpsRedirection();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
    // 以下設定等同 ALL
    // ForwardedHeaders = ForwardedHeaders.XForwardedFor |
    //                    ForwardedHeaders.XForwardedProto |
    //                    ForwardedHeaders.XForwardedHost
});

app.UseHealthChecks("/health");

// app.UseW3CLogging();

app.UseHttpLogging();

var apiVersionDescriptions =
    app.Services
       .GetRequiredService<IApiVersionDescriptionProvider>()
       .ApiVersionDescriptions;

app.MapOpenApi()
   .CacheOutput();

app.UseSwaggerUI(options =>
{
    foreach (var description in apiVersionDescriptions)
    {
        options.SwaggerEndpoint(
            $"/openapi/{description.GroupName}.json",
            $"dotNet web Api {description.GroupName}");
    }

    options.OAuthClientId(authOptions?.ClientId);
    options.OAuthClientSecret(authOptions?.ClientSecret);
    options.OAuthScopeSeparator(" ");
    options.OAuthUsePkce();
});

foreach (var description in apiVersionDescriptions)
{
    app.UseReDoc(options =>
    {
        options.SpecUrl($"/swagger/{description.GroupName}/swagger.json");
        options.RoutePrefix = $"redoc-{description.GroupName}";
        options.DocumentTitle = $"{AppDomain.CurrentDomain.FriendlyName} Api {description.GroupName}";
    });
}

app.MapScalarApiReference(options =>
{
    //not tested yet
    options.AddPreferredSecuritySchemes("OAuth2") // Security scheme name from the OpenAPI document
           .AddAuthorizationCodeFlow("OAuth2", configureFlow =>
           {
               configureFlow.ClientId = authOptions?.ClientId;
               configureFlow.SelectedScopes = ["profile"];
           });
});

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.MapControllers();

app.Run();