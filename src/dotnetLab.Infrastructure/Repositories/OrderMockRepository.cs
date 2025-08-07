using System;
using System.Threading.Tasks;
using dotnetLab.Application.Orders.Ports.Out;
using dotnetLab.Domain.Orders;
using dotnetLab.Domain.Products;

namespace dotnetLab.Infrastructure.Repositories;

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