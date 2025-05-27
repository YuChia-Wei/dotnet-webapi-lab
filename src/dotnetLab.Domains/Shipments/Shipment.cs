using dotnetLab.Domains.Shipments.Entities;
using dotnetLab.Domains.Shipments.ValueObjects;
using dotnetLab.SharedKernel.Aggregates;
using dotnetLab.SharedKernel.Events;

namespace dotnetLab.Domains.Shipments;

/// <summary>
/// 貨運聚合根
/// </summary>
public class Shipment : IAggregateRoot<Guid>
{
    private readonly List<IDomainEvent> _domainEvents = new();
    private readonly List<ShipmentPackage> _packages = new();

    // 供 EF Core 等 ORM 使用的私有建構函式
    private Shipment() { }

    /// <summary>
    /// 建立新的貨運
    /// </summary>
    /// <param name="id">貨運ID</param>
    /// <param name="orderSnapshot">訂單快照</param>
    /// <param name="deliveryAddress">送貨地址</param>
    public Shipment(Guid id, OrderSnapshot orderSnapshot, Address deliveryAddress)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("貨運ID不能為空", nameof(id));
        }

        if (orderSnapshot == null)
        {
            throw new ArgumentNullException(nameof(orderSnapshot));
        }

        if (deliveryAddress == null)
        {
            throw new ArgumentNullException(nameof(deliveryAddress));
        }

        this.Id = id;
        this.OrderDetails = orderSnapshot;
        this.TrackingNumber = this.GenerateTrackingNumber();
        this.Status = ShipmentStatus.Created;
        this.CreatedDate = DateTime.UtcNow;
        this.DeliveryAddress = deliveryAddress;
        this.Version = 1;

        // 從訂單項目快照中計算總重量和是否需要冷藏
        this.CalculateShipmentProperties();

        // 根據訂單項目自動分配包裝
        this.AllocatePackages();
    }

    public OrderSnapshot OrderDetails { get; private set; } // 使用 Value Object 快照
    public string TrackingNumber { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? ShippedDate { get; private set; }
    public DateTime? DeliveredDate { get; private set; }
    public Address DeliveryAddress { get; private set; }
    public decimal TotalWeight { get; private set; }
    public bool RequiresRefrigeration { get; private set; }

    public IReadOnlyCollection<ShipmentPackage> Packages => this._packages.AsReadOnly();

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
    /// 標記貨運為已送達
    /// </summary>
    public void MarkAsDelivered()
    {
        if (this.Status != ShipmentStatus.Shipped && this.Status != ShipmentStatus.OutForDelivery)
        {
            throw new InvalidOperationException($"無法將狀態為 {this.Status} 的貨運標記為已送達");
        }

        this.Status = ShipmentStatus.Delivered;
        this.DeliveredDate = DateTime.UtcNow;

        // 這裡可以添加送達事件
    }

    /// <summary>
    /// 標記貨運為已發貨
    /// </summary>
    public void MarkAsShipped()
    {
        if (this.Status != ShipmentStatus.Created && this.Status != ShipmentStatus.Preparing)
        {
            throw new InvalidOperationException($"無法將狀態為 {this.Status} 的貨運標記為已發貨");
        }

        this.Status = ShipmentStatus.Shipped;
        this.ShippedDate = DateTime.UtcNow;

        // 這裡可以添加發貨事件
    }

    /// <summary>
    /// 更新訂單詳情（當訂單資訊變更時）
    /// </summary>
    /// <param name="updatedOrderSnapshot">更新後的訂單快照</param>
    public void UpdateOrderDetails(OrderSnapshot updatedOrderSnapshot)
    {
        if (updatedOrderSnapshot == null)
        {
            throw new ArgumentNullException(nameof(updatedOrderSnapshot));
        }

        if (updatedOrderSnapshot.OrderId != this.OrderDetails.OrderId)
        {
            throw new InvalidOperationException("無法更新不同訂單的詳情");
        }

        // 只有在特定狀態下才能更新訂單詳情
        if (this.Status != ShipmentStatus.Created && this.Status != ShipmentStatus.Preparing)
        {
            throw new InvalidOperationException($"無法在狀態為 {this.Status} 時更新訂單詳情");
        }

        // 更新訂單快照
        this.OrderDetails = updatedOrderSnapshot;

        // 重新計算貨運屬性
        this.CalculateShipmentProperties();

        // 重新分配包裝
        this._packages.Clear();
        this.AllocatePackages();
    }

    /// <summary>
    /// 根據訂單項目分配包裝
    /// </summary>
    private void AllocatePackages()
    {
        // 簡化的包裝分配邏輯
        // 在實際系統中，這可能是一個複雜的算法，考慮重量、體積、特殊處理要求等

        // 如果需要冷藏，將所有需要冷藏的商品放在一個包裹中
        var refrigeratedItems = this.OrderDetails.OrderLines
                                    .Where(ol => ol.RequiresRefrigeration)
                                    .ToList();

        if (refrigeratedItems.Any())
        {
            var refrigeratedPackage = new ShipmentPackage(
                Guid.NewGuid(),
                this.Id,
                "冷藏包裹",
                refrigeratedItems.Sum(i => i.Weight * i.Quantity),
                true);

            this._packages.Add(refrigeratedPackage);
        }

        // 將其他商品根據重量分配到標準包裹中
        var standardItems = this.OrderDetails.OrderLines
                                .Where(ol => !ol.RequiresRefrigeration)
                                .ToList();

        const decimal maxPackageWeight = 10.0m; // 假設最大包裹重量為10kg

        // 簡單的裝箱算法（實際系統可能使用更複雜的算法）
        var currentWeight = 0.0m;
        var currentItems = new List<OrderLineSnapshot>();

        foreach (var item in standardItems)
        {
            var itemWeight = item.Weight * item.Quantity;

            if (currentWeight + itemWeight > maxPackageWeight && currentItems.Any())
            {
                // 建立新包裹
                var package = new ShipmentPackage(
                    Guid.NewGuid(),
                    this.Id,
                    $"標準包裹 {this._packages.Count + 1}",
                    currentWeight,
                    false);

                this._packages.Add(package);

                // 重置計數
                currentWeight = itemWeight;
                currentItems = new List<OrderLineSnapshot>
                {
                    item
                };
            }
            else
            {
                currentWeight += itemWeight;
                currentItems.Add(item);
            }
        }

        // 添加最後一個包裹（如果有）
        if (currentItems.Any())
        {
            var package = new ShipmentPackage(
                Guid.NewGuid(),
                this.Id,
                $"標準包裹 {this._packages.Count + 1}",
                currentWeight,
                false);

            this._packages.Add(package);
        }
    }

    /// <summary>
    /// 從訂單項目快照中計算貨運屬性
    /// </summary>
    private void CalculateShipmentProperties()
    {
        this.TotalWeight = this.OrderDetails.OrderLines.Sum(ol => ol.Weight * ol.Quantity);
        this.RequiresRefrigeration = this.OrderDetails.OrderLines.Any(ol => ol.RequiresRefrigeration);
    }

    /// <summary>
    /// 生成追蹤號碼
    /// </summary>
    private string GenerateTrackingNumber()
    {
        // 簡化的追蹤號碼生成邏輯
        return $"TN-{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}";
    }
}

/// <summary>
/// 貨運狀態枚舉
/// </summary>
public enum ShipmentStatus
{
    Created,
    Preparing,
    Shipped,
    OutForDelivery,
    Delivered,
    Failed,
    Returned
}