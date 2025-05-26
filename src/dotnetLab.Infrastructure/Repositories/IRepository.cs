using dotnetLab.Infrastructure.Entities;
using dotnetLab.Infrastructure.Specifications;

namespace dotnetLab.Infrastructure.Repositories;

/// <summary>
/// 表示存儲庫模式的介面
/// 存儲庫模式用於封裝數據訪問邏輯，提供統一的數據訪問介面
/// </summary>
/// <typeparam name="T">存儲庫管理的實體類型</typeparam>
/// <typeparam name="TId">實體識別碼的類型</typeparam>
public interface IRepository<T, TId> where T : IDomainEntity<TId>
{
    /// <summary>
    /// 根據識別碼查找實體
    /// </summary>
    /// <param name="id">實體識別碼</param>
    /// <returns>找到的實體，如果未找到則為 null</returns>
    Task<T?> FindByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根據規格查找實體
    /// </summary>
    /// <param name="specification">查詢規格</param>
    /// <returns>符合規格的實體集合</returns>
    Task<IEnumerable<T>> FindAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加實體
    /// </summary>
    /// <param name="entity">要添加的實體</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新實體
    /// </summary>
    /// <param name="entity">要更新的實體</param>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刪除實體
    /// </summary>
    /// <param name="entity">要刪除的實體</param>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}
