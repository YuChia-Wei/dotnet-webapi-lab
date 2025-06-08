namespace dotnetLab.UseCases.Shipments.Queries;

/// <summary>
/// 取得貨運查詢
/// </summary>
public class GetShipmentQuery
{
    /// <summary>
    /// 貨運識別碼
    /// </summary>
    public Guid ShipmentId { get; set; }
}
