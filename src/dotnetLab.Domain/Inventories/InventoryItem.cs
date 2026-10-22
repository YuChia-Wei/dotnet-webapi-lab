using ArchPrecept.Contracts.Domain;

namespace dotnetLab.Domain.Inventories;

/// <summary>
/// 庫存項目實體
/// </summary>
public class InventoryItem : DomainEntity<Guid>
{

    private InventoryItem() { }

    internal InventoryItem(Guid id, Guid productId, int quantity)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("庫存項目ID不能為空", nameof(id));
        }

        if (productId == Guid.Empty)
        {
            throw new ArgumentException("商品ID不能為空", nameof(productId));
        }

        if (quantity < 0)
        {
            throw new ArgumentException("數量不能為負", nameof(quantity));
        }

        this.Id = id;
        this.ProductId = productId;
        this.Quantity = quantity;
    }

    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }






    internal void AdjustQuantity(int amount)
    {
        var newQuantity = this.Quantity + amount;
        if (newQuantity < 0)
        {
            throw new InvalidOperationException("庫存數量不足");
        }

        this.Quantity = newQuantity;
    }
}