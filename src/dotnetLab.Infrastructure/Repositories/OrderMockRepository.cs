using ArchPrecept.Contracts.Application;
using dotnetLab.Domain.Orders;
using dotnetLab.Domain.Products;

namespace dotnetLab.Infrastructure.Repositories;

/// <summary>
/// Order mock repository
/// </summary>
internal class OrderMockRepository : IAggregateRepository<Order, Guid>
{
    private readonly Dictionary<Guid, Order> _saved = new();

    /// <inheritdoc />
    public Task<Order?> FindByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (orderId == Guid.Empty) return Task.FromResult<Order?>(null);
        if (this._saved.TryGetValue(orderId, out var saved)) return Task.FromResult<Order?>(saved);
        var order = new Order(orderId, "Mock Customer");
        var productA = new Product(Guid.NewGuid(), "Product A", 10m, 1.0m, "10x10x10", false);
        var productB = new Product(Guid.NewGuid(), "Product B", 20m, 2.0m, "20x20x20", false);
        order.AddOrderLine(productA, 2);
        order.AddOrderLine(productB, 1);
        return Task.FromResult<Order?>(order);
    }

    /// <summary>在目前 scope 的示範儲存庫保留變更；不提供程序外持久化。</summary>
    public Task SaveAsync(Order aggregate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        this._saved[aggregate.Id] = aggregate;
        return Task.CompletedTask;
    }
}