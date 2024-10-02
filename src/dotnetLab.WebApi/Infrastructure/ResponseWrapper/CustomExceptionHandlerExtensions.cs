using System.Diagnostics;
using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;

namespace dotnetLab.WebApi.Infrastructure.ResponseWrapper;

public static class CustomExceptionHandlerExtensions
{
    public static void UseExceptionWrapper(this WebApplication webApplication)
    {
        webApplication.UseExceptionHandler(applicationBuilder =>
        {
            applicationBuilder.Run(async context =>
            {
                var traceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();

                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;
                var failResultViewModel = new ApiResponse<ApiErrorInformation>
                {
                    Id = traceId,
                    ApiVersion = context.ApiVersioningFeature().RawRequestedApiVersion,
                    RequestPath = $"{context.Request.Path}.{context.Request.Method}",
                    ResponseCode = "Error",
                    Data = new ApiErrorInformation
                    {
                        Message = exception?.Message ?? "unknown error",
                        Description = ExceptionMessage(webApplication, exception)
                    }
                };

                await context.Response.WriteAsJsonAsync(failResultViewModel);
            });
        });
    }

    private static string ExceptionMessage(WebApplication webApplication, Exception? exception)
    {
        return (webApplication.Environment.IsDevelopment() ? exception?.ToString() : exception?.Message) ??
               "unknown error";
    }
}