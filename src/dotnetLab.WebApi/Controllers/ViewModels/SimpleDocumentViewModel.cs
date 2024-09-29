namespace dotnetLab.WebApi.Controllers.ViewModels;

/// <summary>
/// 簡易文件資料
/// </summary>
public class SimpleDocumentViewModel
{
    /// <summary>
    /// 文件資料序號
    /// </summary>
    /// <example>1234</example>
    public int SerialId { get; set; }

    /// <summary>
    /// 文件敘述
    /// </summary>
    /// <example>i'm description</example>
    public string? Description { get; set; }

    /// <summary>
    /// 文件編號
    /// </summary>
    /// <example>doc.111</example>
    public string DocumentNum { get; set; } = string.Empty;
}