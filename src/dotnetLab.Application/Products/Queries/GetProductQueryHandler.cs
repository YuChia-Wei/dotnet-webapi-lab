using dotnetLab.Application.Products.Dtos;
using dotnetLab.Application.Products.UseCases;

namespace dotnetLab.Application.Products.Queries;

/// <summary>將 Wolverine 查詢訊息轉接至應用程式 Use Case。</summary>
public sealed class GetProductQueryHandler(IGetProductUseCase useCase)
{
    /// <summary>分派查詢並傳遞取消權杖。</summary>
    public Task<ProductDto?> HandleAsync(GetProductQuery request, CancellationToken cancellationToken) =>
        useCase.ExecuteAsync(new GetProductInput(request.ProductId), cancellationToken);
}