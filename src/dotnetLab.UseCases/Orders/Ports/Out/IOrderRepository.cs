using dotnetLab.Domains.Orders;

namespace dotnetLab.UseCases.Orders.Ports.Out;

/// <summary>
/// 訂單 Repository
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// 依識別碼取得訂單
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    Task<Order?> GetAsync(Guid orderId);

    /// <summary>
    /// 儲存訂單
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    Task<Guid> SaveAsync(Order order);
}