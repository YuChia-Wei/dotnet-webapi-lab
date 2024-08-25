using System.Diagnostics;
using dotnetLab.WebApi.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dotnetLab.WebApi.Infrastructure.Attributes;

public class ApiResponseWrappingFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        // 確保只在成功的情況下進行包裝
        if (context.Result is ObjectResult objectResult && objectResult.StatusCode >= 200 && objectResult.StatusCode < 300)
        {
            // var traceId = context.HttpContext.Request.Headers["X-Trace-Id"].ToString() ?? "no-trace-id";

            var traceId = Activity.Current?.TraceId.ToString() ?? "no-trace-id";

            var wrappedResponse = new ApiResponse<object>
            {
                Id = traceId,
                Success = true,
                Message = "Request successful",
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