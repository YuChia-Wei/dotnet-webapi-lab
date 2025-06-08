using dotnetLab.Domains.Products;

namespace dotnetLab.UseCases.Products.Ports.Out;

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