using System.Net.Mime;
using dotnetLab.WebApi.Controllers.ViewModels;
using Microsoft.AspNetCore.Diagnostics;

namespace dotnetLab.WebApi.Infrastructure.Middlewares;

public static class CustomExceptionHandlerExtensions
{
    public static void UseCustomExceptionHandler(this WebApplication webApplication)
    {
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