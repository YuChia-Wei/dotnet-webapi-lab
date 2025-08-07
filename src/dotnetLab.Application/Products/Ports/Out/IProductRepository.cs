using dotnetLab.Domain.Products;

namespace dotnetLab.Application.Products.Ports.Out;

/// <summary>
/// 商品 Repository
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// 取得商品
    /// </summary>
    Task<Product?> GetAsync(Guid productId);
}