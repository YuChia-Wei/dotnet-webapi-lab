namespace dotnetLab.WebApi.Infrastructure.ResponseWrapper;

/// <summary>
/// 例外類別的擴充
/// </summary>
public static class ExceptionExtensions
{
    public static ApiErrorInformation GetApiErrorInformation(this Exception exception)
    {
        // 建立包含錯誤資訊的 api response 物件
        var apiErrorInformation = new ApiErrorInformation
        {
            ErrorCode = exception.GetApiErrorCode(),
            Message = exception.Message,
            Description = IsDevelopment() ? exception.ToString() : exception.Message
        };

        return apiErrorInformation;
    }

    /// <summary>
    /// get api error code
    /// </summary>
    /// <param name="exception"></param>
    /// <returns>custom api error code, return "err-unknown" by default</returns>
    private static string GetApiErrorCode(this Exception exception)
    {
        if (exception is ApiErrorException errorException)
        {
            return errorException.ErrorCode;
        }

        return "err-unknown";
    }

    private static bool IsDevelopment()
    {
        var currentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var isDevelopment = string.Equals(currentEnvironment, Environments.Development, StringComparison.OrdinalIgnoreCase);
        return isDevelopment;
    }
}

public class ApiErrorException : Exception
{
    public string ErrorCode { get; } = "err-unknown";
}