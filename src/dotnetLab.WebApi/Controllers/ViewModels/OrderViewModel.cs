namespace dotnetLab.WebApi.Controllers.ViewModels;

/// <summary>
/// 訂單資料
/// </summary>
public class OrderViewModel
{
    /// <summary>
    /// 訂單識別碼
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 客戶名稱
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 訂單日期
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; }
}
