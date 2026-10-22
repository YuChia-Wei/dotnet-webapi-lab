using dotnetLab.Application.Orders.Dtos;
using dotnetLab.Application.Orders.UseCases;

namespace dotnetLab.Application.Orders.Queries;

/// <summary>將 Wolverine 查詢訊息轉接至應用程式 Use Case。</summary>
public sealed class GetOrderQueryHandler(IGetOrderUseCase useCase)
{
    /// <summary>分派查詢並傳遞取消權杖。</summary>
    public Task<OrderDto?> HandleAsync(GetOrderQuery request, CancellationToken cancellationToken) =>
        useCase.ExecuteAsync(new GetOrderInput(request.OrderId), cancellationToken);
}