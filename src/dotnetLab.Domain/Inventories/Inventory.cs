using ArchPrecept.Contracts.Domain;

namespace dotnetLab.Domain.Inventories;

/// <summary>
/// 庫存聚合根
/// </summary>
public class Inventory : AggregateRoot<Guid>
{
    private readonly List<InventoryItem> _items = new();

    private Inventory() { }

    /// <summary>建立指定識別碼的庫存聚合。</summary>
    public Inventory(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("庫存ID不能為空", nameof(id));
        }

        this.Id = id;
        this.Version = 1;
    }

    public IReadOnlyCollection<InventoryItem> Items => this._items.AsReadOnly();







    /// <summary>
    /// 調整庫存
    /// </summary>
    public void AdjustItem(Guid productId, int amount)
    {
        var item = this._items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
        {
            if (amount < 0)
            {
                throw new InvalidOperationException("庫存數量不足");
            }

            item = new InventoryItem(Guid.NewGuid(), productId, amount);
            this._items.Add(item);
            return;
        }

        item.AdjustQuantity(amount);
    }
}