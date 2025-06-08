namespace dotnetLab.UseCases.Products.Dtos;

/// <summary>
/// 商品資料
/// </summary>
public class ProductDto
{
    /// <summary>
    /// 識別碼
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 單價
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 重量
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// 尺寸
    /// </summary>
    public string Dimensions { get; set; } = string.Empty;

    /// <summary>
    /// 是否需要冷藏
    /// </summary>
    public bool RequiresRefrigeration { get; set; }
}
