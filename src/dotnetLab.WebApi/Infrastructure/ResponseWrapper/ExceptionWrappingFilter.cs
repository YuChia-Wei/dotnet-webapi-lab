using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dotnetLab.WebApi.Infrastructure.ResponseWrapper;

/// <summary>
/// 例外處理
/// </summary>
/// <remarks>
/// https://marcus116.blogspot.com/2021/06/aspnet-core-exception-handling.html
/// https://medium.com/vx-company/centralize-your-net-exception-handling-with-filters-a1e0fccf17b8
/// </remarks>
public class ExceptionWrappingFilter : IExceptionFilter
{
    /// <summary>
    /// Called after an action has thrown an <see cref="T:System.Exception" />.
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ExceptionContext" />.</param>
    public void OnException(ExceptionContext context)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ExceptionWrappingFilter>>();

        // 這邊可以設定你的 exception 要不要繼續往外拋，如果設定為 true，則外面會收到 200 ok，並且不會觸發 exception hendler
        // 這會讓 open telemetry 等觀測工具看不出例外，但是這端看系統是如何定義錯誤處理的規則，並沒有哪一種比較好
        context.ExceptionHandled = context.Exception is ShouldBeHandledException;

        // 取得該次錯誤時的追蹤編號以便設定在 error information 中
        var traceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();

        var exception = context.Exception;

        logger.LogError("Exception occured: {ExceptionMessage} , Exception Description: {ExceptionDescription} ",
                        exception.Message,
                        exception.ToString());

        var failResultViewModel = new ApiResponse<ApiErrorInformation>
        {
            Id = traceId,
            ApiVersion = context.HttpContext.ApiVersioningFeature().RawRequestedApiVersion,
            RequestPath = $"{context.HttpContext.Request.Path}.{context.HttpContext.Request.Method}",

            // 利用擴充方法來將例外資料轉為專用的錯誤回應資訊
            Data = exception.GetApiErrorInformation()
        };

        context.Result = new JsonResult(failResultViewModel)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}