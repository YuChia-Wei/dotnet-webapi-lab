using ArchPrecept.Contracts.Application;
using dotnetLab.Domain.Products;
using dotnetLab.Domain.Shipments;
using dotnetLab.Domain.Shipments.ValueObjects;

namespace dotnetLab.Infrastructure.Repositories;

/// <summary>
/// Shipment mock repository
/// </summary>
internal class ShipmentMockRepository : IAggregateRepository<Shipment, Guid>
{
    private readonly Dictionary<Guid, Shipment> _saved = new();

    /// <inheritdoc />
    public Task<Shipment?> FindByIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (shipmentId == Guid.Empty) return Task.FromResult<Shipment?>(null);
        if (this._saved.TryGetValue(shipmentId, out var saved)) return Task.FromResult<Shipment?>(saved);
        var address = new Address("Street 1", "City", "State", "00000", "Country", "Tester", "0912345678");
        var product = new Product(Guid.NewGuid(), "Product A", 10m, 1.0m, "10x10x10", false);
        var orderLine = new OrderLineSnapshot(
            product.Id,
            product.Name,
            1,
            product.Weight,
            product.Dimensions,
            product.RequiresRefrigeration);
        var orderSnapshot = new OrderSnapshot(
            shipmentId,
            "Mock Customer",
            DateTime.UtcNow,
            new[]
            {
                orderLine
            });
        var shipment = new Shipment(shipmentId, orderSnapshot, address);
        return Task.FromResult<Shipment?>(shipment);
    }

    /// <summary>在目前 scope 的示範儲存庫保留變更；不提供程序外持久化。</summary>
    public Task SaveAsync(Shipment aggregate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        this._saved[aggregate.Id] = aggregate;
        return Task.CompletedTask;
    }
}