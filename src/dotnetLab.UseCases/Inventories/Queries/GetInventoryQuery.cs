namespace dotnetLab.UseCases.Inventories.Queries;

/// <summary>
/// 取得庫存查詢
/// </summary>
public class GetInventoryQuery
{
    /// <summary>
    /// 庫存識別碼
    /// </summary>
    public Guid InventoryId { get; set; }
}
