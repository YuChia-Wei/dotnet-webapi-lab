using dotnetLab.BuildingBlocks.Domain;
using dotnetLab.Domain.Orders.Events;

namespace dotnetLab.Domain.Orders;

/// <summary>
/// 訂單項目實體
/// </summary>
public class OrderLine : IDomainEntity<Guid>
{
    private readonly List<IDomainEvent> _domainEvents = new();

    // 供 EF Core 等 ORM 使用的私有建構函式
    private OrderLine() { }

    internal OrderLine(Guid orderId, Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException("訂單ID不能為空", nameof(orderId));
        }

        if (productId == Guid.Empty)
        {
            throw new ArgumentException("產品ID不能為空", nameof(productId));
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException("產品名稱不能為空", nameof(productName));
        }

        if (unitPrice <= 0)
        {
            throw new ArgumentException("單價必須大於零", nameof(unitPrice));
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("數量必須大於零", nameof(quantity));
        }

        this.Id = Guid.NewGuid();
        this.OrderId = orderId;
        this.ProductId = productId;
        this.ProductName = productName;
        this.UnitPrice = unitPrice;
        this.Quantity = quantity;

        // 建立一個領域事件，但由於 OrderLine 是 Entity，不是 Aggregate Root，
        // 所以這個事件會被加到 Order (Aggregate Root) 的事件集合中
        var @event = new OrderLineAdded(this.OrderId, this.ProductId, this.ProductName, this.UnitPrice, this.Quantity);
        this.AddDomainEvent(@event);
    }

    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => this.UnitPrice * this.Quantity;

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

    /// <summary>
    /// 修改訂單項目數量
    /// </summary>
    internal void ChangeQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
        {
            throw new ArgumentException("數量必須大於零", nameof(newQuantity));
        }

        var oldQuantity = this.Quantity;
        this.Quantity = newQuantity;

        // 建立數量變更事件
        var @event = new OrderLineQuantityChanged(this.OrderId, this.Id, oldQuantity, newQuantity);
        this.AddDomainEvent(@event);
    }
}