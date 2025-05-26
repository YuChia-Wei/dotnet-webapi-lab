using dotnetLab.Infrastructure.Events;

namespace dotnetLab.Infrastructure.Entities;

/// <summary>
/// 表示領域實體的介面
/// 領域實體是具有唯一識別碼的物件
/// </summary>
/// <typeparam name="TId">實體識別碼的類型</typeparam>
public interface IDomainEntity<TId> where TId : notnull
{
    /// <summary>
    /// 取得實體的識別碼
    /// </summary>
    TId Id { get; }

    /// <summary>
    /// 取得實體的領域事件
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// 添加領域事件
    /// </summary>
    /// <param name="domainEvent">要添加的領域事件</param>
    void AddDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// 移除領域事件
    /// </summary>
    /// <param name="domainEvent">要移除的領域事件</param>
    void RemoveDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// 清除所有領域事件
    /// </summary>
    void ClearDomainEvents();
}
