using ArchPrecept.Contracts.Application;
using dotnetLab.Domain.Inventories;

namespace dotnetLab.Infrastructure.Repositories;

/// <summary>
/// Inventory mock repository
/// </summary>
internal class InventoryMockRepository : IAggregateRepository<Inventory, Guid>
{
    private readonly Dictionary<Guid, Inventory> _saved = new();

    /// <inheritdoc />
    public Task<Inventory?> FindByIdAsync(Guid inventoryId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (inventoryId == Guid.Empty) return Task.FromResult<Inventory?>(null);
        if (this._saved.TryGetValue(inventoryId, out var saved)) return Task.FromResult<Inventory?>(saved);
        var inventory = new Inventory(inventoryId);
        inventory.AdjustItem(Guid.NewGuid(), 10);
        return Task.FromResult<Inventory?>(inventory);
    }
    /// <summary>在目前 scope 的示範儲存庫保留變更；不提供程序外持久化。</summary>
    public Task SaveAsync(Inventory aggregate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        this._saved[aggregate.Id] = aggregate;
        return Task.CompletedTask;
    }
}