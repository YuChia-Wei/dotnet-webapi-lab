using ArchPrecept.Contracts.Domain;

namespace dotnetLab.Domain.Shipments.ValueObjects;

/// <summary>
/// 訂單項目快照 Value Object
/// 這是一個不可變的值物件，代表訂單項目的關鍵資訊
/// </summary>
public sealed class OrderLineSnapshot : ValueObject
{
    /// <summary>建立並驗證OrderLineSnapshot值物件。</summary>
    public OrderLineSnapshot(
        Guid productId,
        string productName,
        int quantity,
        decimal weight,
        string dimensions,
        bool requiresRefrigeration)
    {
        this.ProductId = productId;
        this.ProductName = productName;
        this.Quantity = quantity;
        this.Weight = weight;
        this.Dimensions = dimensions;
        this.RequiresRefrigeration = requiresRefrigeration;

        this.ValidateSelf();
    }

    public Guid ProductId { get; }
    public string ProductName { get; }
    public int Quantity { get; }

    // 僅供物流系統需要的特定屬性
    public decimal Weight { get; }
    public string Dimensions { get; }
    public bool RequiresRefrigeration { get; }

    /// <summary>
    /// 驗證訂單項目快照資料的有效性
    /// </summary>
    private void ValidateSelf()
    {
        if (this.ProductId == Guid.Empty)
        {
            throw new ArgumentException("產品ID不能為空", nameof(this.ProductId));
        }

        if (string.IsNullOrWhiteSpace(this.ProductName))
        {
            throw new ArgumentException("產品名稱不能為空", nameof(this.ProductName));
        }

        if (this.Quantity <= 0)
        {
            throw new ArgumentException("數量必須大於零", nameof(this.Quantity));
        }

        if (this.Weight <= 0)
        {
            throw new ArgumentException("重量必須大於零", nameof(this.Weight));
        }

        if (string.IsNullOrWhiteSpace(this.Dimensions))
        {
            throw new ArgumentException("尺寸不能為空", nameof(this.Dimensions));
        }
    }

    /// <inheritdoc />
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.ProductId;
        yield return this.ProductName;
        yield return this.Quantity;
        yield return this.Weight;
        yield return this.Dimensions;
        yield return this.RequiresRefrigeration;
    }
}