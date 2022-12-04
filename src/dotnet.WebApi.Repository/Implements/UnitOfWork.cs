using System.Collections;
using dotnet.WebApi.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace dotnet.WebApi.Repository.Implements;

/// <summary>
/// UnitOfWork
/// </summary>
public class UnitOfWork : IUnitOfWork, IDisposable
{
    /// <summary>
    /// Context
    /// </summary>
    private readonly DbContext _context;

    /// <summary>
    /// The disposed
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// The repositories
    /// </summary>
    private Hashtable _repositories;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public UnitOfWork(DbContext context)
    {
        this._context = context;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting
    /// unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 取得泛型 Repository
    /// </summary>
    /// <typeparam name="TEntity">實體</typeparam>
    /// <returns></returns>
    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        this._repositories ??= new Hashtable();

        var type = typeof(TEntity);

        if (!this._repositories.ContainsKey(type))
        {
            this._repositories[type] = new Repository<TEntity>(this._context);
        }

        return (IRepository<TEntity>)this._repositories[type];
    }

    /// <summary>
    /// 儲存資料變更
    /// </summary>
    /// <returns></returns>
    public async Task<int> SaveChangesAsync()
    {
        return await this._context.SaveChangesAsync();
    }

    /// <summary>
    /// Commits the transaction. (非必要勿使用，須配合 commit/Rollback Transaction 使用)
    /// </summary>
    public void CommitTransaction()
    {
        this._context.Database.CommitTransaction();
    }

    /// <summary>
    /// Begins the transaction. (非必要勿使用，須配合 commit/Rollback Transaction 使用)
    /// </summary>
    public void BeginTransaction()
    {
        this._context.Database.BeginTransaction();
    }

    /// <summary>
    /// Rollbacks the transaction. (非必要勿使用，須配合 commit/Rollback Transaction 使用)
    /// </summary>
    public void RollbackTransaction()
    {
        this._context.Database.RollbackTransaction();
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
    /// only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this._context.Dispose();
            }
        }

        this._disposed = true;
    }
}