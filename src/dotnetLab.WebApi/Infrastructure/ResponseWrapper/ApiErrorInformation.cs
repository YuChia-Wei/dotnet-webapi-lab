using System.Text.Json.Serialization;

namespace dotnetLab.WebApi.Infrastructure.ResponseWrapper;

/// <summary>
/// api error information
/// </summary>
public class ApiErrorInformation
{
    /// <summary>
    /// 錯誤代號
    /// </summary>
    public string ErrorCode { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 錯誤詳細說明
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Description { get; set; }
}