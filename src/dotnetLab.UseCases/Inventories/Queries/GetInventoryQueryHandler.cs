using dotnetLab.UseCases.Inventories.Dtos;
using dotnetLab.UseCases.Inventories.Ports.Out;

namespace dotnetLab.UseCases.Inventories.Queries;

/// <summary>
/// 取得庫存處理程序
/// </summary>
public class GetInventoryQueryHandler
{
    private readonly IInventoryRepository _inventoryRepository;

    /// <summary>
    /// ctor
    /// </summary>
    public GetInventoryQueryHandler(IInventoryRepository inventoryRepository)
    {
        this._inventoryRepository = inventoryRepository;
    }

    /// <summary>
    /// 取得庫存資料
    /// </summary>
    public async ValueTask<InventoryDto?> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
    {
        var inventory = await this._inventoryRepository.GetAsync(request.InventoryId);
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