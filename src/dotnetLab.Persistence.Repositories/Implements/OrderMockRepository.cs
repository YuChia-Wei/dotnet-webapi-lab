using dotnetLab.Domains.Orders;
using dotnetLab.Domains.Products;
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
