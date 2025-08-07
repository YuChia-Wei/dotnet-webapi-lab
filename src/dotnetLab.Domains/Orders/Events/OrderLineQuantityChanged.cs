using dotnetLab.BuildingBlocks.Events;

namespace dotnetLab.Domains.Orders.Events;

/// <summary>
/// 訂單項目數量變更事件
/// </summary>
public record OrderLineQuantityChanged(Guid OrderId, Guid OrderLineId, int OldQuantity, int NewQuantity) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}