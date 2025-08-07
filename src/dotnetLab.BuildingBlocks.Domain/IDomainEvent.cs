namespace dotnetLab.BuildingBlocks.Domain;

/// <summary>
/// 表示領域事件的介面
/// 領域事件用於捕獲領域中發生的重要事情
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// 事件發生的時間
    /// </summary>
    DateTime OccurredOn { get; }
}
