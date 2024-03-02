namespace dotnetLab.Database.SampleDb.Entities;

/// <summary>
/// 範例資料
/// </summary>
public class SimpleDocument
{
    /// <summary>
    /// 序號
    /// </summary>
    public int SerialId { get; set; }

    /// <summary>
    /// 敘述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 文件編號
    /// </summary>
    public string DocumentNum { get; set; } = string.Empty;
}