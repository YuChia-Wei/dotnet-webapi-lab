using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using dotnet.WebApi.Common.OptionModels;
using dotnet.WebApi.Infrastructure.Authorization.Policy;
using dotnet.WebApi.Infrastructure.CustomJsonConverter;
using dotnet.WebApi.Infrastructure.SwaggerFilters;
using dotnet.WebApi.Repository.Db.SampleDb;
using HealthChecks.Prometheus.Metrics;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Host
       .UseSerilog((context, configuration) =>
       {
           configuration.ReadFrom.Configuration(context.Configuration);
           configuration.Enrich.WithProperty("ApplicationName",
                                             AppDomain.CurrentDomain.FriendlyName ?? "app name not found.");
           configuration.Enrich.WithProperty("MACHINENAME",
                                             Environment.GetEnvironmentVariable("MACHINENAME") ??
                                             Environment.MachineName);
       });

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
    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
});

//使用 w3c 格式的 log，用來輸出類似 IIS Log 的內容
//檔案預設存放於 {application path}/logs/ 資料夾下
//在 docker 裡面就是在 /app/logs 這個位置；依據本專案中的 dockerfile 設定
builder.Services.AddW3CLogging(logging =>
{
    // Log all W3C fields
    logging.LoggingFields = W3CLoggingFields.All;
    // 5 MB
    logging.FileSizeLimit = 5 * 1024 * 1024;
    logging.RetainedFileCountLimit = 2;
    logging.FileName = AppDomain.CurrentDomain.FriendlyName ??
                       Environment.MachineName;
    logging.FlushInterval = TimeSpan.FromSeconds(2);

    //.net 7 new feature
    logging.AdditionalRequestHeaders.Add("x-forwarded-for");
});

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
}).AddApiExplorer(options =>
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
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "dotnet.WebApi V1", Version = "v1" });

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
                    AuthorizationUrl = new Uri($"{authOptions.Authority}/connect/authorize"),
                    TokenUrl = new Uri($"{authOptions.Authority}/connect/token"),
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

builder.Services.AddLogging();

// 註冊 EF Core Db Context
builder.Services.AddDbContext<SampleDbContext>(
    (provider, builder1) =>
    {
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

        builder1.UseLoggerFactory(loggerFactory)
                .UseSqlServer("connection-string")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    },
    ServiceLifetime.Transient,
    ServiceLifetime.Singleton);

// MediatR
var serviceAssembly = AppDomain.CurrentDomain.Load("dotnet.WebApi.Service");
var webApiAssembly = AppDomain.CurrentDomain.Load("dotnet.WebApi");
builder.Services.AddMediatR(webApiAssembly, serviceAssembly);

// builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

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
           options.Authority = authOptions.Authority;
           options.RequireHttpsMetadata = false;
           options.Audience = authOptions.Audience;
       });

builder.Services
       .AddAuthorization(options =>
       {
           options.AddPolicy(nameof(LoginUserRequestedPolicy), LoginUserRequestedPolicy.PolicyAction());
       });

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// 純 Web Api 專案不建議使用此設定
// via https://docs.microsoft.com/zh-tw/aspnet/core/security/enforcing-ssl?view=aspnetcore-6.0&tabs=visual-studio
// app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHealthChecks(
       "/metrics",
       new HealthCheckOptions
       {
           ResponseWriter = PrometheusResponseWriter.WritePrometheusResultText, AllowCachingResponses = false
       })
   .UseHealthChecks(
       "/health",
       new HealthCheckOptions
       {
           ResultStatusCodes =
           {
               [HealthStatus.Healthy] = StatusCodes.Status200OK,
               [HealthStatus.Degraded] = StatusCodes.Status200OK,
               [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
           },
           AllowCachingResponses = false,
           Predicate = _ => true,
           ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
       });

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

       options.OAuthClientId(authOptions.ClientId);
       options.OAuthClientSecret(authOptions.ClientSecret);
       options.OAuthScopeSeparator(" ");
       options.OAuthUsePkce();
   });

foreach (var description in apiVersionDescriptions)
{
    app.UseReDoc(options =>
    {
        options.SpecUrl($"/swagger/{description.GroupName}/swagger.json");
        options.RoutePrefix = $"redoc-{description.GroupName}";
        options.DocumentTitle = $"dotNet Sample Api {description.GroupName}";
    });
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseW3CLogging();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapControllers();
});

app.Run();