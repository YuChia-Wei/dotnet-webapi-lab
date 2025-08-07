using System;
using dotnetLab.BuildingBlocks.Domain;

namespace dotnetLab.Domain.Orders.Events;

/// <summary>
/// 訂單項目添加事件
/// </summary>
public record OrderLineAdded(Guid OrderId, Guid ProductId, string ProductName, decimal UnitPrice, int Quantity) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}