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
    private readonly ILogger<ExceptionWrappingFilter> _logger;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="logger"></param>
    public ExceptionWrappingFilter(ILogger<ExceptionWrappingFilter> logger)
    {
        this._logger = logger;
    }

    /// <summary>
    /// Called after an action has thrown an <see cref="T:System.Exception" />.
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ExceptionContext" />.</param>
    public void OnException(ExceptionContext context)
    {
        // 這邊可以設定你的 exception 要不要繼續往外拋，若設定為 true 且並未在這個 exception filter 中處理回應資料的話，會導致 api 回應 200 ok，並且不會觸發 exception handler
        // 這邊如何處理端看團隊是如何定義錯誤處理的規則，並沒有哪一種比較好
        context.ExceptionHandled = true;

        this._logger.LogError("Exception occured: {ExceptionMessage} , Exception Description: {ExceptionDescription} ",
                              context.Exception.Message,
                              context.Exception.ToString());

        var failResultViewModel = context.ParseToFailResultViewModel();

        context.Result = new JsonResult(failResultViewModel)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}