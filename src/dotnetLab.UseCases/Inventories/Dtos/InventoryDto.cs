namespace dotnetLab.UseCases.Inventories.Dtos;

/// <summary>
/// 庫存資料
/// </summary>
public class InventoryDto
{
    /// <summary>
    /// 商品識別碼
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    public int Quantity { get; set; }
}