using dotnetLab.BuildingBlocks.Domain;

namespace dotnetLab.Domain.Inventories;

/// <summary>
/// 庫存聚合根
/// </summary>
public class Inventory : IAggregateRoot<Guid>
{
    private readonly List<IDomainEvent> _domainEvents = new();
    private readonly List<InventoryItem> _items = new();

    private Inventory() { }

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

    public Guid Id { get; private set; }

    public int Version { get; private set; }

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