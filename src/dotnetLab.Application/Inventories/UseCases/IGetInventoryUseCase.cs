using dotnetLab.Application.Inventories.Dtos;

namespace dotnetLab.Application.Inventories.UseCases;

/// <summary>取得Inventory的業務輸入。</summary>
public sealed record GetInventoryInput(Guid InventoryId);

/// <summary>提供Inventory查詢的應用程式邊界。</summary>
public interface IGetInventoryUseCase
{
    /// <summary>查詢資料；找不到時回傳 null。</summary>
    Task<InventoryDto?> ExecuteAsync(GetInventoryInput input, CancellationToken cancellationToken);
}