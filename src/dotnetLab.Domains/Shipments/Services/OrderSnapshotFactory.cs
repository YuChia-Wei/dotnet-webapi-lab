using dotnetLab.Domains.Orders;
using dotnetLab.Domains.Shipments.ValueObjects;

namespace dotnetLab.Domains.Shipments.Services;

/// <summary>
/// 訂單快照工廠
/// 負責將 Order 聚合根轉換為 Shipment 所需的 OrderSnapshot Value Object
/// </summary>
public class OrderSnapshotFactory
{
    // 產品服務的介面，用於獲取物流相關的產品資訊
    private readonly IProductInfoService _productInfoService;

    public OrderSnapshotFactory(IProductInfoService productInfoService)
    {
        this._productInfoService = productInfoService ?? throw new ArgumentNullException(nameof(productInfoService));
    }

    /// <summary>
    /// 建立訂單快照
    /// </summary>
    /// <param name="order">原始訂單</param>
    /// <returns>訂單快照 Value Object</returns>
    public OrderSnapshot Create(Order order)
    {
        // 轉換訂單項目為訂單項目快照
        var orderLineSnapshots = order.OrderLines.Select(ol =>
        {
            // 從產品服務獲取物流所需的特定產品資訊
            var productInfo = this._productInfoService.GetProductInfo(ol.ProductId);

            return new OrderLineSnapshot(
                ol.ProductId,
                ol.ProductName,
                ol.Quantity,
                productInfo.Weight,
                productInfo.Dimensions,
                productInfo.RequiresRefrigeration);
        }).ToList();

        // 建立並返回訂單快照
        return new OrderSnapshot(
            order.Id,
            order.CustomerName,
            order.OrderDate,
            orderLineSnapshots);
    }
}

/// <summary>
/// 產品資訊服務介面
/// </summary>
public interface IProductInfoService
{
    ProductLogisticsInfo GetProductInfo(Guid productId);
}

/// <summary>
/// 產品物流資訊
/// </summary>
public class ProductLogisticsInfo
{
    public decimal Weight { get; set; }
    public string Dimensions { get; set; }
    public bool RequiresRefrigeration { get; set; }
}