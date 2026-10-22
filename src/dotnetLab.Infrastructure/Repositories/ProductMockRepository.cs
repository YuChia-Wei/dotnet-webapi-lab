using ArchPrecept.Contracts.Application;
using dotnetLab.Domain.Products;

namespace dotnetLab.Infrastructure.Repositories;

/// <summary>
/// Product mock repository
/// </summary>
internal class ProductMockRepository : IAggregateRepository<Product, Guid>
{
    private readonly Dictionary<Guid, Product> _saved = new();

    /// <inheritdoc />
    public Task<Product?> FindByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (productId == Guid.Empty) return Task.FromResult<Product?>(null);
        if (this._saved.TryGetValue(productId, out var saved)) return Task.FromResult<Product?>(saved);
        var product = new Product(productId, "Mock Product", 100m, 1.0m, "10x10x10", false);
        return Task.FromResult<Product?>(product);
    }
    /// <summary>在目前 scope 的示範儲存庫保留變更；不提供程序外持久化。</summary>
    public Task SaveAsync(Product aggregate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        this._saved[aggregate.Id] = aggregate;
        return Task.CompletedTask;
    }
}