using System.Diagnostics;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dotnetLab.WebApi.Infrastructure.ResponseWrapper;

/// <summary>
/// 成功執行的 api 回應包裝器
/// </summary>
public class ApiResponseWrappingFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (IsBinaryResult(context))
        {
            return;
        }

        if (TryGetSuccessResultData(context, out var successObjectResult))
        {
            // var traceId = context.HttpContext.Request.Headers["X-Trace-Id"].ToString() ?? "no-trace-id";

            var traceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();

            var wrappedResponse = new ApiResponse<object?>
            {
                Id = traceId,
                ApiVersion = context.HttpContext.ApiVersioningFeature().RawRequestedApiVersion,
                RequestPath = $"{context.HttpContext.Request.Path}.{context.HttpContext.Request.Method}",
                Data = successObjectResult?.Value
            };

            context.Result = new ObjectResult(wrappedResponse)
            {
                Formatters = successObjectResult?.Formatters,
                ContentTypes = successObjectResult?.ContentTypes,
                StatusCode = successObjectResult?.StatusCode,
                DeclaredType = successObjectResult?.DeclaredType
            };
        }

        if (TryGetBadRequestObjectResultData(context, out var badRequestObjectResult))
        {
            var apiErrorInformation = new ApiErrorInformation();
            switch (badRequestObjectResult.Value)
            {
                // FluentValidation 的錯誤物件
                case List<ValidationFailure> validationFailures:
                    apiErrorInformation.ErrorCode = validationFailures.First().ErrorCode;
                    apiErrorInformation.Message = validationFailures.First().ErrorMessage;
                    break;

                // DataAnnotations 的錯誤物件
                case ValidationProblemDetails validationProblemDetails:
                    apiErrorInformation.ErrorCode = validationProblemDetails.Title;
                    // 這邊需要額外進入 values 裡面才能取得真的要的錯誤訊息
                    apiErrorInformation.Message = validationProblemDetails.Errors.Values.FirstOrDefault().FirstOrDefault();
                    break;

                case ApiErrorInformation apiError:
                    apiErrorInformation = apiError;
                    break;
            }

            var traceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();

            var wrappedResponse = new ApiResponse<ApiErrorInformation>
            {
                Id = traceId,
                ApiVersion = context.HttpContext.ApiVersioningFeature().RawRequestedApiVersion,
                RequestPath = $"{context.HttpContext.Request.Path}.{context.HttpContext.Request.Method}",
                Data = apiErrorInformation
            };

            context.Result = new BadRequestObjectResult(wrappedResponse)
            {
                Formatters = badRequestObjectResult.Formatters,
                ContentTypes = badRequestObjectResult.ContentTypes,
                StatusCode = badRequestObjectResult.StatusCode,
                DeclaredType = badRequestObjectResult.DeclaredType
            };
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        // 這裡可以加入在結果執行後的處理邏輯
    }

    private static bool IsBinaryResult(ResultExecutingContext context)
    {
        return context.Result is FileResult ||
               context.HttpContext.Response.ContentType?.StartsWith("image/") == true ||
               context.HttpContext.Response.ContentType == "application/octet-stream";
    }

    private static bool TryGetBadRequestObjectResultData(ResultExecutingContext context, out BadRequestObjectResult? badRequestResult)
    {
        if (context.Result is BadRequestObjectResult result)
        {
            badRequestResult = result;
            return true;
        }

        badRequestResult = null;
        return false;
    }

    private static bool TryGetSuccessResultData(ResultExecutingContext context, out ObjectResult? successObjectResult)
    {
        switch (context.Result)
        {
            case OkObjectResult okResult:
                successObjectResult = okResult;
                return true;
            case ObjectResult { StatusCode: >= 200 and < 300 } objectResult:
                successObjectResult = objectResult;
                return true;
            default:
                successObjectResult = null;
                return false;
        }
    }
}