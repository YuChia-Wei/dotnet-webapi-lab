using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace dotnet.WebApi.Repository.Interfaces;

/// <summary>
/// 泛型 Repository 介面
/// </summary>
/// <typeparam name="TEntity">實體</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// 實體集合
    /// </summary>
    DbSet<TEntity> Entities { get; }

    /// <summary>
    /// 加入實體
    /// </summary>
    /// <param name="entity">實體</param>
    void Add(TEntity entity);

    /// <summary>
    /// 加入實體 (非同步，特定情境才使用)
    /// </summary>
    /// <param name="entity">實體</param>
    /// <param name="cancellationToken">取消 Token</param>
    /// <returns></returns>
    Task<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// 加入實體 (非同步，特定情境才使用)
    /// </summary>
    /// <param name="entity">實體</param>
    /// <returns></returns>
    Task<EntityEntry<TEntity>> AddAsync(TEntity entity);

    /// <summary>
    /// 新增多筆實體
    /// </summary>
    /// <param name="entities">實體集合</param>
    void AddRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// 新增多筆實體
    /// </summary>
    /// <param name="entities">實體集合</param>
    /// <param name="cancellationToken">取消 Token</param>
    /// <returns></returns>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    /// <summary>
    /// 新增多筆實體
    /// </summary>
    /// <param name="entities">實體集合</param>
    /// <returns></returns>
    Task AddRangeAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// 取得資料
    /// </summary>
    /// <param name="disableTracking">是否關閉 tracking</param>
    /// <returns></returns>
    IQueryable<TEntity> Get(bool disableTracking);

    /// <summary>
    /// 取得資料
    /// </summary>
    /// <returns></returns>
    IQueryable<TEntity> Get();

    /// <summary>
    /// 刪除實體
    /// </summary>
    /// <param name="entity">實體</param>
    void Remove(TEntity entity);

    /// <summary>
    /// 刪除多筆實體
    /// </summary>
    /// <param name="entities">實體集合</param>
    void RemoveRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// 更新實體
    /// </summary>
    /// <param name="entity">實體</param>
    void Update(TEntity entity);

    /// <summary>
    /// 更新多筆實體
    /// </summary>
    /// <param name="entities">實體集合</param>
    void UpdateRange(IEnumerable<TEntity> entities);
}