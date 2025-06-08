using dotnetLab.Domains.Shipments;

namespace dotnetLab.UseCases.Shipments.Ports.Out;

/// <summary>
/// 貨運 Repository
/// </summary>
public interface IShipmentRepository
{
    /// <summary>
    /// 依識別碼取得貨運
    /// </summary>
    /// <param name="shipmentId"></param>
    /// <returns></returns>
    Task<Shipment?> GetAsync(Guid shipmentId);

    /// <summary>
    /// 儲存貨運
    /// </summary>
    /// <param name="shipment"></param>
    /// <returns></returns>
    Task<Guid> SaveAsync(Shipment shipment);
}
