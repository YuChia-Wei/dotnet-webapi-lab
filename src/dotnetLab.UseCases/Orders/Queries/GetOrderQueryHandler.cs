using dotnetLab.UseCases.Orders.Dtos;
using dotnetLab.UseCases.Orders.Ports.Out;

namespace dotnetLab.UseCases.Orders.Queries;

/// <summary>
/// 取得訂單處理程序
/// </summary>
public class GetOrderQueryHandler
{
    private readonly IOrderRepository _orderRepository;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="orderRepository"></param>
    public GetOrderQueryHandler(IOrderRepository orderRepository)
    {
        this._orderRepository = orderRepository;
    }

    /// <summary>
    /// 取得訂單
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async ValueTask<OrderDto?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await this._orderRepository.GetAsync(request.OrderId);
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
