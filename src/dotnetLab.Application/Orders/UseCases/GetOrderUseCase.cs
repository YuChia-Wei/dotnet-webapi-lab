using ArchPrecept.Contracts.Application;
using dotnetLab.Application.Orders.Dtos;
using dotnetLab.Domain.Orders;

namespace dotnetLab.Application.Orders.UseCases;

/// <summary>
/// 取得訂單處理程序
/// </summary>
public sealed class GetOrderUseCase : IGetOrderUseCase
{
    private readonly IAggregateRepository<Order, Guid> _orderRepository;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="orderRepository"></param>
    public GetOrderUseCase(IAggregateRepository<Order, Guid> orderRepository)
    {
        this._orderRepository = orderRepository;
    }

    /// <summary>
    /// 取得訂單
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OrderDto?> ExecuteAsync(GetOrderInput input, CancellationToken cancellationToken)
    {
        var order = await this._orderRepository.FindByIdAsync(input.OrderId, cancellationToken);
        if (order == null)
        {
            return null;
        }

        var dto = new OrderDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount
        };

        return dto;
    }
}