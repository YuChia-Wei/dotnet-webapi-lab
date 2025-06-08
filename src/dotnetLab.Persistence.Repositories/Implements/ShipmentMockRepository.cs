using dotnetLab.Domains.Shipments;
using dotnetLab.Domains.Shipments.ValueObjects;
using dotnetLab.Persistence.Repositories.Implements;
using dotnetLab.UseCases.Shipments.Ports.Out;

namespace dotnetLab.Persistence.Repositories.Implements;

/// <summary>
/// Shipment mock repository
/// </summary>
public class ShipmentMockRepository : IShipmentRepository
{
    /// <inheritdoc />
    public Task<Shipment?> GetAsync(Guid shipmentId)
    {
        var address = new Address("Street 1", "City", "State", "00000", "Country", "Tester", "0912345678");
        var orderLine = new OrderLineSnapshot(
            Guid.NewGuid(),
            "Product A",
            1,
            1.0m,
            "10x10x10",
            false);
        var orderSnapshot = new OrderSnapshot(
            shipmentId,
            "Mock Customer",
            DateTime.UtcNow,
            new[] { orderLine });
        var shipment = new Shipment(shipmentId, orderSnapshot, address);
        return Task.FromResult<Shipment?>(shipment);
    }

    /// <inheritdoc />
    public Task<Guid> SaveAsync(Shipment shipment)
    {
        return Task.FromResult(shipment.Id);
    }
}
