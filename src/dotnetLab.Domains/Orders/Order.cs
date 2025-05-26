using dotnetLab.Domains.Orders.Events;
using dotnetLab.Infrastructure.Aggregates;
using dotnetLab.Infrastructure.Events;

namespace dotnetLab.Domains.Orders;

/// <summary>
/// 訂單聚合根
/// </summary>
public class Order : IAggregateRoot<Guid>
{
    private readonly List<IDomainEvent> _domainEvents = new();
    private readonly List<OrderLine> _orderLines = new();

    // 供 EF Core 等 ORM 使用的私有建構函式
    private Order() { }

    /// <summary>
    /// 建立訂單
    /// </summary>
    public Order(Guid id, string customerName)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("訂單ID不能為空", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentException("客戶名稱不能為空", nameof(customerName));
        }

        this.Id = id;
        this.CustomerName = customerName;
        this.OrderDate = DateTime.UtcNow;
        this.TotalAmount = 0;
        this.Version = 1;
    }

    public string CustomerName { get; private set; }
    public DateTime OrderDate { get; private set; }
    public decimal TotalAmount { get; private set; }

    public IReadOnlyCollection<OrderLine> OrderLines => this._orderLines.AsReadOnly();

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
    /// 添加訂單項目
    /// </summary>
    public void AddOrderLine(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        // 檢查是否已存在相同產品的訂單項目
        if (this._orderLines.Any(ol => ol.ProductId == productId))
        {
            throw new InvalidOperationException($"產品 {productName} 已存在於訂單中");
        }

        // 建立新的訂單項目
        var orderLine = new OrderLine(this.Id, productId, productName, unitPrice, quantity);
        this._orderLines.Add(orderLine);

        // 從 Entity 收集領域事件，並加入到聚合根的事件集合中
        foreach (var @event in orderLine.DomainEvents)
        {
            this.AddDomainEvent(@event);
        }

        orderLine.ClearDomainEvents();

        // 這是 Aggregate 內部的同步處理：
        // 當添加新訂單項目時，自動更新訂單總金額
        this.UpdateTotalAmount();
    }

    /// <summary>
    /// 修改訂單項目數量
    /// </summary>
    public void ChangeOrderLineQuantity(Guid orderLineId, int newQuantity)
    {
        var orderLine = this._orderLines.FirstOrDefault(ol => ol.Id == orderLineId);
        if (orderLine == null)
        {
            throw new InvalidOperationException($"訂單項目 {orderLineId} 不存在");
        }

        orderLine.ChangeQuantity(newQuantity);

        // 從 Entity 收集領域事件，並加入到聚合根的事件集合中
        foreach (var @event in orderLine.DomainEvents)
        {
            this.AddDomainEvent(@event);

            // 處理訂單項目數量變更事件 - 這是關鍵的「聚合內部同步處理」
            if (@event is OrderLineQuantityChanged quantityChanged)
            {
                // 當訂單項目數量變更時，立即更新訂單總金額
                this.UpdateTotalAmount();
            }
        }

        orderLine.ClearDomainEvents();
    }

    /// <summary>
    /// 移除訂單項目
    /// </summary>
    public void RemoveOrderLine(Guid orderLineId)
    {
        var orderLine = this._orderLines.FirstOrDefault(ol => ol.Id == orderLineId);
        if (orderLine == null)
        {
            throw new InvalidOperationException($"訂單項目 {orderLineId} 不存在");
        }

        this._orderLines.Remove(orderLine);

        // 當移除訂單項目時，自動更新訂單總金額
        this.UpdateTotalAmount();
    }

    /// <summary>
    /// 更新訂單總金額
    /// 這是一個私有方法，用於聚合內部同步處理
    /// </summary>
    private void UpdateTotalAmount()
    {
        this.TotalAmount = this._orderLines.Sum(ol => ol.TotalPrice);
    }
}