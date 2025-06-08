using dotnetLab.Domains.Inventories;
using dotnetLab.UseCases.Inventories.Ports.Out;

namespace dotnetLab.Persistence.Repositories.Implements;

/// <summary>
/// Inventory mock repository
/// </summary>
public class InventoryMockRepository : IInventoryRepository
{
    /// <inheritdoc />
    public Task<Inventory?> GetAsync(Guid inventoryId)
    {
        var inventory = new Inventory(inventoryId);
        inventory.AdjustItem(Guid.NewGuid(), 10);
        return Task.FromResult<Inventory?>(inventory);
    }
}