using dotnetLab.Domains.Orders;
using dotnetLab.Domains.Products;
using dotnetLab.UseCases.Orders.Ports.Out;

namespace dotnetLab.Persistence.Repositories.Implements;

/// <summary>
/// Order mock repository
/// </summary>
public class OrderMockRepository : IOrderRepository
{
    /// <inheritdoc />
    public Task<Order?> GetAsync(Guid orderId)
    {
        var order = new Order(orderId, "Mock Customer");
        var productA = new Product(Guid.NewGuid(), "Product A", 10m, 1.0m, "10x10x10", false);
        var productB = new Product(Guid.NewGuid(), "Product B", 20m, 2.0m, "20x20x20", false);
        order.AddOrderLine(productA, 2);
        order.AddOrderLine(productB, 1);
        return Task.FromResult<Order?>(order);
    }

    /// <inheritdoc />
    public Task<Guid> SaveAsync(Order order)
    {
        return Task.FromResult(order.Id);
    }
}