using System;
using System.Threading.Tasks;
using dotnetLab.Application.Inventories.Ports.Out;
using dotnetLab.Domain.Inventories;

namespace dotnetLab.Infrastructure.Repositories;

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