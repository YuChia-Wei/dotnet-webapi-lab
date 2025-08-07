namespace dotnetLab.WebApi.Controllers.ViewModels;

/// <summary>
/// 貨運資料
/// </summary>
public class ShipmentViewModel
{
    /// <summary>
    /// 貨運識別碼
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 追蹤號碼
    /// </summary>
    public string TrackingNumber { get; set; } = string.Empty;

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = string.Empty;
}