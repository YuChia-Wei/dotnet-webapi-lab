using ArchPrecept.Contracts.Domain;

namespace dotnetLab.Domain.Orders.Events;

/// <summary>
/// 訂單項目添加事件
/// </summary>
public record OrderLineAdded(Guid OrderId, Guid ProductId, string ProductName, decimal UnitPrice, int Quantity) : IDomainEvent
{
    /// <summary>事件的唯一識別碼。</summary>
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>事件發生的 UTC 時間。</summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}