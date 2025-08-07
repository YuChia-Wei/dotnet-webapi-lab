namespace dotnetLab.BuildingBlocks.Domain;

/// <summary>
/// 表示聚合根的介面
/// 聚合根是領域模型中的一個概念，它代表一個由多個相關實體組成的集合，並負責確保這些實體的一致性
/// </summary>
/// <typeparam name="TId">聚合根識別碼的類型</typeparam>
public interface IAggregateRoot<TId> : IDomainEntity<TId>
{
    /// <summary>
    /// 聚合根的版本，用於處理並發控制
    /// </summary>
    int Version { get; }
}
