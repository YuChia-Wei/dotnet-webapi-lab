using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dotnetLab.WebApi.Infrastructure.ResponseWrapper;

/// <summary>
/// ExceptionContext 的例外擴充
/// </summary>
public static class ExceptionContextExtensions
{
    /// <summary>
    /// 解析 ExceptionContext 中的錯誤資訊並轉為 api response 物件
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static ApiResponse<ApiErrorInformation> ParseToFailResultViewModel(this ExceptionContext context)
    {
        return new ApiResponse<ApiErrorInformation>
        {
            // 取得該次錯誤時的追蹤編號以便設定在 error information 中
            Id = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString(),
            ApiVersion = context.HttpContext.ApiVersioningFeature().RawRequestedApiVersion,
            RequestPath = $"{context.HttpContext.Request.Path}.{context.HttpContext.Request.Method}",

            // 利用擴充方法來將例外資料轉為專用的錯誤回應資訊
            Data = context.Exception.GetApiErrorInformation()
        };
    }
}