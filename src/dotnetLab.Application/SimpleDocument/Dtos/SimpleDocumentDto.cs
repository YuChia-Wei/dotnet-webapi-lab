namespace dotnetLab.Application.SimpleDocument.Dtos;

/// <summary>
/// 簡易文件資料
/// </summary>
public class SimpleDocumentDto
{
    /// <summary>
    /// 文件資料序號
    /// </summary>
    public int SerialId { get; set; }

    /// <summary>
    /// 文件敘述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 文件編號
    /// </summary>
    public string DocumentNum { get; set; } = string.Empty;
}