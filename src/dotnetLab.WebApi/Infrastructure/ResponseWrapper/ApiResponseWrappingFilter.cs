using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dotnetLab.WebApi.Infrastructure.ResponseWrapper;

public class ApiResponseWrappingFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        // 確保只在成功的情況下進行包裝
        if (context.Result is ObjectResult { StatusCode: >= 200 and < 300 } objectResult)
        {
            // var traceId = context.HttpContext.Request.Headers["X-Trace-Id"].ToString() ?? "no-trace-id";

            var traceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();

            var wrappedResponse = new ApiResponse<object?>
            {
                Id = traceId,
                ApiVersion = context.HttpContext.ApiVersioningFeature().RawRequestedApiVersion,
                RequestPath = $"{context.HttpContext.Request.Path}.{context.HttpContext.Request.Method}",
                ResponseCode = "SUCCESS",
                Data = objectResult.Value
            };

            context.Result = new ObjectResult(wrappedResponse) { StatusCode = objectResult.StatusCode, DeclaredType = objectResult.DeclaredType };
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        // 這裡可以加入在結果執行後的處理邏輯
    }
}