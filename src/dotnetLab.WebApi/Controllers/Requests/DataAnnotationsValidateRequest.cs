using System.ComponentModel.DataAnnotations;

namespace dotnetLab.WebApi.Controllers.Requests;

/// <summary>
/// DataAnnotations 驗證請求類別。
/// </summary>
public class DataAnnotationsValidateRequest
{
    /// <summary>
    /// 獲取或設定 SerialId，此屬性代表序列識別碼。
    /// 其值應位於 1 至 10 個字元之間。
    /// </summary>
    /// <value>
    /// 序列識別碼，需符合長度限制條件（三至十個字元）。
    /// </value>
    [MaxLength(10)]
    [MinLength(1)]
    public string SerialId { get; set; }
}