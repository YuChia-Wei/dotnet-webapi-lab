namespace dotnet.WebApi.Repository.Interfaces;

/// <summary>
/// UnitOfWork 介面
/// </summary>
/// <seealso cref="System.IDisposable" />
public interface IUnitOfWork
{
    /// <summary>
    /// Begins the transaction. (非必要勿使用，須配合 commit/Rollback Transaction 使用)
    /// </summary>
    void BeginTransaction();

    /// <summary>
    /// Commits the transaction. (非必要勿使用，須配合 commit/Rollback Transaction 使用)
    /// </summary>
    void CommitTransaction();

    /// <summary>
    /// 取得泛型 Repository
    /// </summary>
    /// <typeparam name="TEntity">實體</typeparam>
    /// <returns></returns>
    IRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class;

    /// <summary>
    /// Rollbacks the transaction. (非必要勿使用，須配合 commit/Rollback Transaction 使用)
    /// </summary>
    void RollbackTransaction();

    /// <summary>
    /// 儲存資料變更
    /// </summary>
    /// <returns></returns>
    Task<int> SaveChangesAsync();
}