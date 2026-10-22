using ArchPrecept.Contracts.Application;
using dotnetLab.Application.Products.Dtos;
using dotnetLab.Domain.Products;

namespace dotnetLab.Application.Products.UseCases;

/// <summary>
/// 取得商品處理程序
/// </summary>
public sealed class GetProductUseCase : IGetProductUseCase
{
    private readonly IAggregateRepository<Product, Guid> _productRepository;

    /// <summary>
    /// ctor
    /// </summary>
    public GetProductUseCase(IAggregateRepository<Product, Guid> productRepository)
    {
        this._productRepository = productRepository;
    }

    /// <summary>
    /// 取得商品
    /// </summary>
    public async Task<ProductDto?> ExecuteAsync(GetProductInput input, CancellationToken cancellationToken)
    {
        var product = await this._productRepository.FindByIdAsync(input.ProductId, cancellationToken);
        if (product == null)
        {
            return null;
        }


        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            UnitPrice = product.UnitPrice,
            Weight = product.Weight,
            Dimensions = product.Dimensions,
            RequiresRefrigeration = product.RequiresRefrigeration
        };

        return dto;
    }
}