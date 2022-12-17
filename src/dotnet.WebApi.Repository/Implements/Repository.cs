using dotnet.WebApi.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace dotnet.WebApi.Repository.Implements;

/// <summary>
/// 泛型 Repository
/// </summary>
/// <typeparam name="TEntity">實體</typeparam>
/// <seealso cref="IRepository{TEntity}" />
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// The database context
    /// </summary>
    private readonly DbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{TEntity}" /> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public Repository(DbContext dbContext)
    {
        this._dbContext = dbContext;
        this.Entities = this._dbContext.Set<TEntity>();
    }

    /// <summary>
    /// 實體集合
    /// </summary>
    public DbSet<TEntity> Entities { get; }

    /// <summary>
    /// 加入實體
    /// </summary>
    /// <param name="entity">實體</param>
    public void Add(TEntity entity)
    {
        this.Entities.Add(entity);
    }

    /// <summary>
    /// 加入實體 (非同步，特定情境才使用)
    /// </summary>
    /// <param name="entity">實體</param>
    /// <returns></returns>
    public Task<EntityEntry<TEntity>> AddAsync(TEntity entity)
    {
        return this.AddAsync(entity, default);
    }

    /// <summary>
    /// 加入實體 (非同步，特定情境才使用)
    /// </summary>
    /// <param name="entity">實體</param>
    /// <param name="cancellationToken">取消 Token</param>
    /// <returns></returns>
    public async Task<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        return await this.Entities.AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// 新增多筆實體
    /// </summary>
    /// <param name="entities">實體集合</param>
    public void AddRange(IEnumerable<TEntity> entities)
    {
        this.Entities.AddRange(entities);
    }

    /// <summary>
    /// 新增多筆實體
    /// </summary>
    /// <param name="entities">實體集合</param>
    /// <returns></returns>
    public Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        return this.AddRangeAsync(entities, default);
    }

    /// <summary>
    /// 新增多筆實體
    /// </summary>
    /// <param name="entities">實體集合</param>
    /// <param name="cancellationToken">取消 Token</param>
    /// <returns></returns>
    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        return this.Entities.AddRangeAsync(entities, cancellationToken);
    }

    /// <summary>
    /// 取得資料 (預設無追蹤)
    /// </summary>
    /// <returns></returns>
    public IQueryable<TEntity> Get()
    {
        return this.Get(true);
    }

    /// <summary>
    /// 取得資料
    /// </summary>
    /// <param name="disableTracking">是否關閉 tracking</param>
    /// <returns></returns>
    public IQueryable<TEntity> Get(bool disableTracking)
    {
        return disableTracking ? this.Entities.AsNoTracking() : this.Entities;
    }

    /// <summary>
    /// 刪除實體
    /// </summary>
    /// <param name="entity">實體</param>
    public void Remove(TEntity entity)
    {
        this.Entities.Remove(entity);
    }

    /// <summary>
    /// 刪除多筆實體
    /// </summary>
    /// <param name="entities">實體集合</param>
    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        this.Entities.RemoveRange(entities);
    }

    /// <summary>
    /// 更新實體
    /// </summary>
    /// <param name="entity">實體</param>
    public void Update(TEntity entity)
    {
        this.Entities.Update(entity);
    }

    /// <summary>
    /// 更新多筆實體
    /// </summary>
    /// <param name="entities">實體集合</param>
    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        this.Entities.UpdateRange(entities);
    }
}