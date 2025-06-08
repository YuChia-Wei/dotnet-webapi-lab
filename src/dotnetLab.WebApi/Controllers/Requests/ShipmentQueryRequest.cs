namespace dotnetLab.WebApi.Controllers.Requests;

/// <summary>
/// 取得貨運 Request
/// </summary>
public class ShipmentQueryRequest
{
    /// <summary>
    /// 貨運識別碼
    /// </summary>
    public Guid ShipmentId { get; set; }
}
