using ArchPrecept.Contracts.Application;
using dotnetLab.Application.Inventories.Dtos;
using dotnetLab.Domain.Inventories;

namespace dotnetLab.Application.Inventories.UseCases;

/// <summary>
/// 取得庫存處理程序
/// </summary>
public sealed class GetInventoryUseCase : IGetInventoryUseCase
{
    private readonly IAggregateRepository<Inventory, Guid> _inventoryRepository;

    /// <summary>
    /// ctor
    /// </summary>
    public GetInventoryUseCase(IAggregateRepository<Inventory, Guid> inventoryRepository)
    {
        this._inventoryRepository = inventoryRepository;
    }

    /// <summary>
    /// 取得庫存資料
    /// </summary>
    public async Task<InventoryDto?> ExecuteAsync(GetInventoryInput input, CancellationToken cancellationToken)
    {
        var inventory = await this._inventoryRepository.FindByIdAsync(input.InventoryId, cancellationToken);
        if (inventory == null)
        {
            return null;
        }

        var firstItem = inventory.Items.FirstOrDefault();
        if (firstItem == null)
        {
            return null;
        }

        var dto = new InventoryDto
        {
            ProductId = firstItem.ProductId,
            Quantity = firstItem.Quantity
        };

        return dto;
    }
}