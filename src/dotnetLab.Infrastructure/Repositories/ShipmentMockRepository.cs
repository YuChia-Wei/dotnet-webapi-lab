using dotnetLab.Application.Shipments.Ports.Out;
using dotnetLab.Domain.Products;
using dotnetLab.Domain.Shipments;
using dotnetLab.Domain.Shipments.ValueObjects;

namespace dotnetLab.Infrastructure.Repositories;

/// <summary>
/// Shipment mock repository
/// </summary>
internal class ShipmentMockRepository : IShipmentRepository
{
    /// <inheritdoc />
    public Task<Shipment?> GetAsync(Guid shipmentId)
    {
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

    /// <inheritdoc />
    public Task<Guid> SaveAsync(Shipment shipment)
    {
        return Task.FromResult(shipment.Id);
    }
}