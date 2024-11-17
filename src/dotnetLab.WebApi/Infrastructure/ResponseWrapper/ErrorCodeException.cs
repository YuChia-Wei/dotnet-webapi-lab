namespace dotnetLab.WebApi.Infrastructure.ResponseWrapper;

/// <summary>
/// 系統內部專用的錯誤例外
/// </summary>
/// <remarks>
/// 如果有使用例外來處理錯誤的話，可以利用這種做法來處理專用錯誤碼
/// </remarks>
public class ErrorCodeException : Exception
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="errorCode"></param>
    public ErrorCodeException(string errorCode = "err-unknown")
    {
        this.ErrorCode = errorCode;
    }

    /// <summary>
    /// error code
    /// </summary>
    public string ErrorCode { get; init; }
}