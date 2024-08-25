using System.Net.Mime;
using System.Text.Json;
using dotnetLab.WebApi.Controllers.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace dotnetLab.WebApi.Infrastructure.Middlewares;

public class ApiResponse<T>
{
    public ApiResponse() { }

    public ApiResponse(T data, bool success = true, string message = null)
    {
        this.Success = success;
        this.Message = message;
        this.Data = data;
    }

    public string Id { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}

public class ResponseWrappingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>() == null)
        {
            // 這是控制器動作，所以繼續處理
            await next(context);
            return;
        }

        // 獲取原始的 response body
        var originalBodyStream = context.Response.Body;

        using (var newBodyStream = new MemoryStream())
        {
            context.Response.Body = newBodyStream;

            // 執行下一個 Middleware 或控制器
            await next(context);

            // 重置流位置以便讀取
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            // var bodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();

            // 將回應內容包裝到 ApiResponse 中
            var wrappedResponse = new ApiResponse<object?> { Data = await JsonSerializer.DeserializeAsync<object?>(context.Response.Body) };

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            // 將包裝好的回應重新寫入 response
            await JsonSerializer.SerializeAsync(context.Response.Body, wrappedResponse);
            context.Response.ContentType = "application/json";

            // 將回應寫回原始流
            await newBodyStream.CopyToAsync(originalBodyStream);
        }
    }
}

public static class CustomExceptionHandlerExtensions
{
    public static void UseEazyApiResponseWrapper(this WebApplication webApplication)
    {
        webApplication.UseMiddleware<ResponseWrappingMiddleware>();

        webApplication.UseExceptionHandler(applicationBuilder =>
        {
            applicationBuilder.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                // using static System.Net.Mime.MediaTypeNames;
                context.Response.ContentType = MediaTypeNames.Text.Plain;

                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;
                var failResultViewModel = new ErrorResultViewModel
                {
                    ApiVersion = context.ApiVersioningFeature().RawRequestedApiVersion,
                    RequestPath = $"{context.Request.Path}.{context.Request.Method}",
                    Error = new ErrorInformation
                    {
                        Message = exception.Message,
                        Description = webApplication.Environment.IsDevelopment() ? exception.ToString() : exception.Message
                    }
                };

                await context.Response.WriteAsJsonAsync(failResultViewModel);
            });
        });
    }
}