using dotnetLab.Domains.Products;
using dotnetLab.UseCases.Products.Ports.Out;

namespace dotnetLab.Persistence.Repositories.Implements;

/// <summary>
/// Product mock repository
/// </summary>
public class ProductMockRepository : IProductRepository
{
    /// <inheritdoc />
    public Task<Product?> GetAsync(Guid productId)
    {
        var product = new Product(productId, "Mock Product", 100m, 1.0m, "10x10x10", false);
        return Task.FromResult<Product?>(product);
    }
}