using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dotnetLab.WebApi.Infrastructure.ResponseWrapper;

/// <summary>
///
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
        // var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ExceptionWrappingFilter>>();
        // logger.LogError(context.Exception, context.Exception.Message);

        // 這邊可以設定你的 exception 要不要繼續往外拋，如果設定為 true，則外面會收到 200 ok，並且不會觸發 exception hendler
        // 這會讓 open telemetry 等觀測工具看不出例外，但是這端看系統是如何定義錯誤處理的規則，並沒有哪一種比較好
        context.ExceptionHandled = context.Exception is ShouldBeHandledException;

        // 這邊要自己處理 response 物件與 status code
        // 對比使用 Exception Handler，會發現 exception handler 那邊只需要處理回應資料，不用特別寫 status code
        var httpContextResponse = context.HttpContext.Response;
    }
}

public class ShouldBeHandledException : Exception
{
}