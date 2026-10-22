using dotnetLab.Application.Products.Dtos;

namespace dotnetLab.Application.Products.UseCases;

/// <summary>取得Product的業務輸入。</summary>
public sealed record GetProductInput(Guid ProductId);

/// <summary>提供Product查詢的應用程式邊界。</summary>
public interface IGetProductUseCase
{
    /// <summary>查詢資料；找不到時回傳 null。</summary>
    Task<ProductDto?> ExecuteAsync(GetProductInput input, CancellationToken cancellationToken);
}