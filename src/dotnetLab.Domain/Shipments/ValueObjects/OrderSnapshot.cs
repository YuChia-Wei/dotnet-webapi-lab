using dotnetLab.BuildingBlocks.Domain;

namespace dotnetLab.Domain.Shipments.ValueObjects;

/// <summary>
/// 訂單快照 Value Object
/// 這是一個不可變的值物件，代表訂單的關鍵資訊
/// </summary>
public record OrderSnapshot : ValueObjectBase
{
    public OrderSnapshot(
        Guid orderId,
        string customerName,
        DateTime orderDate,
        IEnumerable<OrderLineSnapshot> orderLines)
    {
        this.OrderId = orderId;
        this.CustomerName = customerName;
        this.OrderDate = orderDate;
        this.OrderLines = orderLines.ToList().AsReadOnly();

        this.ValidateSelf();
    }

    public Guid OrderId { get; }
    public string CustomerName { get; }
    public DateTime OrderDate { get; }
    public IReadOnlyCollection<OrderLineSnapshot> OrderLines { get; }

    /// <summary>
    /// 驗證訂單快照資料的有效性
    /// </summary>
    protected override void ValidateSelf()
    {
        if (IsEmpty(this.OrderId))
        {
            throw new ArgumentException("訂單ID不能為空", nameof(this.OrderId));
        }

        if (IsNullOrWhitespace(this.CustomerName))
        {
            throw new ArgumentException("客戶名稱不能為空", nameof(this.CustomerName));
        }

        if (this.OrderDate == default)
        {
            throw new ArgumentException("訂單日期不能為預設值", nameof(this.OrderDate));
        }

        if (this.OrderLines == null || !this.OrderLines.Any())
        {
            throw new ArgumentException("訂單項目不能為空", nameof(this.OrderLines));
        }
    }
}