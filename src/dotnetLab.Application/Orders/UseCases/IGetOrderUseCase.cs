using dotnetLab.Application.Orders.Dtos;

namespace dotnetLab.Application.Orders.UseCases;

/// <summary>取得Order的業務輸入。</summary>
public sealed record GetOrderInput(Guid OrderId);

/// <summary>提供Order查詢的應用程式邊界。</summary>
public interface IGetOrderUseCase
{
    /// <summary>查詢資料；找不到時回傳 null。</summary>
    Task<OrderDto?> ExecuteAsync(GetOrderInput input, CancellationToken cancellationToken);
}