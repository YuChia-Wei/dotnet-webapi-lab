using dotnetLab.BuildingBlocks.Events;
using dotnetLab.UseCases.Products.Dtos;
using dotnetLab.UseCases.Products.Ports.Out;

namespace dotnetLab.UseCases.Products.Queries;

/// <summary>
/// 取得商品處理程序
/// </summary>
public class GetProductQueryHandler
{
    private readonly IProductRepository _productRepository;

    /// <summary>
    /// ctor
    /// </summary>
    public GetProductQueryHandler(IProductRepository productRepository)
    {
        this._productRepository = productRepository;
    }

    /// <summary>
    /// 取得商品
    /// </summary>
    public async ValueTask<ProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await this._productRepository.GetAsync(request.ProductId);
        if (product == null)
        {
            return null;
        }

        var domainEvents = product.DomainEvents.Where(o => o is ProductionDomainEvent);

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

public record ProductionDomainEvent : IDomainEvent
{
    /// <summary>
    /// 事件發生的時間
    /// </summary>
    public DateTime OccurredOn { get; }
}