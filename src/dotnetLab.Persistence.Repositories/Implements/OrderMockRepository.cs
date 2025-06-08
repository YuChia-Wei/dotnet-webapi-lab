using dotnetLab.Domains.Orders;
using dotnetLab.Persistence.Repositories.Implements;
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
        order.AddOrderLine(Guid.NewGuid(), "Product A", 10m, 2);
        order.AddOrderLine(Guid.NewGuid(), "Product B", 20m, 1);
        return Task.FromResult<Order?>(order);
    }

    /// <inheritdoc />
    public Task<Guid> SaveAsync(Order order)
    {
        return Task.FromResult(order.Id);
    }
}
