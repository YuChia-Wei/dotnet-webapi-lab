using dotnetLab.SharedKernel.Entities;
using dotnetLab.SharedKernel.Events;

namespace dotnetLab.Domains.Inventories;

/// <summary>
/// 庫存項目實體
/// </summary>
public class InventoryItem : IDomainEntity<Guid>
{
    private readonly List<IDomainEvent> _domainEvents = new();

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

    public Guid Id { get; private set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => this._domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        this._domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        this._domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        this._domainEvents.Clear();
    }

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