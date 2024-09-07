﻿using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning.Conventions;
using dotnetLab.Authorize.OptionModels;
using dotnetLab.Repository;
using dotnetLab.UseCase;
using dotnetLab.WebApi.Infrastructure.Authorization.Policy;
using dotnetLab.WebApi.Infrastructure.CustomJsonConverter;
using dotnetLab.WebApi.Infrastructure.Middlewares;
using dotnetLab.WebApi.Infrastructure.SwaggerFilters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

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

// 設定 Swagger (OpenApi 文件內容)
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = $"{AppDomain.CurrentDomain.FriendlyName} V1", Version = "v1" });

    //Swagger OAuth Setting
    options.AddSecurityDefinition(
        "OAuth2",
        new OpenApiSecurityScheme
        {
            Description = @"Authorization Code, 請先勾選 scope: ",
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"{authOptions.AuthorizationEndpoint}"),
                    TokenUrl = new Uri($"{authOptions.TokenEndpoint}"),
                    Scopes = new Dictionary<string, string> { { authOptions.Audience, "Sample Api" } }
                }
            }
        });

    // 掛載 ExampleFilter
    options.ExampleFilters();

    //Swagger OAuth Setting
    options.OperationFilter<AuthorizeCheckOperationFilter>();

    var basePath = AppContext.BaseDirectory;
    var xmlFiles = Directory.EnumerateFiles(basePath, "*.xml", SearchOption.TopDirectoryOnly);

    foreach (var xmlFile in xmlFiles)
    {
        options.IncludeXmlComments(xmlFile);
    }
});

// 取得所有 Swagger Examples
builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

builder.Services.AddHttpClient();

// 開啟 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// OAuth
builder.Services
       .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
       {
           options.Authority = authOptions.Issuer;
           options.RequireHttpsMetadata = false;
           options.Audience = authOptions.Audience;
       });

builder.Services
       .AddAuthorization(options =>
       {
           options.AddPolicy(nameof(LoginUserRequestedPolicy), LoginUserRequestedPolicy.PolicyAction());
       });

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
app.UseCustomExceptionHandler();

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

var apiVersionDescriptions =
    app.Services
       .GetRequiredService<IApiVersionDescriptionProvider>()
       .ApiVersionDescriptions;

app.UseSwagger()
   .UseSwaggerUI(options =>
   {
       foreach (var description in apiVersionDescriptions)
       {
           options.SwaggerEndpoint(
               $"{description.GroupName}/swagger.json",
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

app.UseRouting();

// app.UseW3CLogging();

// app.UseHttpLogging();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.MapControllers();

app.Run();