namespace dotnetLab.UseCases.Orders.Queries;

/// <summary>
/// 取得訂單查詢
/// </summary>
public class GetOrderQuery
{
    /// <summary>
    /// 訂單識別碼
    /// </summary>
    public Guid OrderId { get; set; }
}