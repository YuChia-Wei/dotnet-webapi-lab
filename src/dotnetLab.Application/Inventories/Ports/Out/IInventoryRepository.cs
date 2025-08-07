using dotnetLab.Domain.Inventories;

namespace dotnetLab.Application.Inventories.Ports.Out;

/// <summary>
/// 庫存 Repository
/// </summary>
public interface IInventoryRepository
{
    /// <summary>
    /// 取得庫存聚合
    /// </summary>
    Task<Inventory?> GetAsync(Guid inventoryId);
}