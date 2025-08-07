using dotnetLab.Application.Products.Ports.Out;
using dotnetLab.Domain.Products;

namespace dotnetLab.Infrastructure.Repositories;

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