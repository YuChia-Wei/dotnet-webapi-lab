using ArchPrecept.Contracts.Domain;
using dotnetLab.Domain.Inventories;
using dotnetLab.Domain.Orders;
using dotnetLab.Domain.Orders.Events;
using dotnetLab.Domain.Products;
using dotnetLab.Domain.SampleDoc;
using dotnetLab.Domain.Shipments;
using dotnetLab.Domain.Shipments.ValueObjects;
using Xunit;

namespace dotnetLab.Tests;

/// <summary>驗證套件遷移後仍成立的領域不變條件。</summary>
public sealed class DomainBehaviorTests
{
    /// <summary>無效商品不能被建立。</summary>
    [Theory]
    [InlineData("id")]
    [InlineData("name")]
    [InlineData("price")]
    [InlineData("weight")]
    [InlineData("dimensions")]
    public void Invalid_product_is_rejected(string invalid)
    {
        Assert.Throws<ArgumentException>(() => new Product(
            invalid == "id" ? Guid.Empty : Guid.NewGuid(), invalid == "name" ? " " : "Milk",
            invalid == "price" ? 0m : 100m, invalid == "weight" ? 0m : 1m,
            invalid == "dimensions" ? " " : "10x10x10", true));
    }

    /// <summary>商品使用已發布的聚合契約，且改價不能破壞識別碼。</summary>
    [Fact]
    public void Product_uses_package_aggregate_and_preserves_price_invariants()
    {
        var product = CreateProduct();
        var id = product.Id;
        Assert.IsAssignableFrom<AggregateRoot<Guid>>(product);
        product.UpdatePrice(120m);
        Assert.Equal(120m, product.UnitPrice);
        Assert.Equal(id, product.Id);
        Assert.Throws<ArgumentException>(() => product.UpdatePrice(0));
        Assert.Equal(120m, product.UnitPrice);
    }

    /// <summary>訂單收集子實體事件、維持總額並可清除待處理事件。</summary>
    [Fact]
    public void Order_collects_entity_events_and_updates_total()
    {
        var product = CreateProduct();
        var order = new Order(Guid.NewGuid(), "Customer");
        order.AddOrderLine(product, 2);
        Assert.Equal(200m, order.TotalAmount);
        var added = Assert.IsType<OrderLineAdded>(Assert.Single(order.DomainEvents));
        Assert.NotEqual(Guid.Empty, added.EventId);
        Assert.Equal(DateTimeKind.Utc, added.OccurredOn.Kind);
        Assert.Empty(Assert.Single(order.OrderLines).DomainEvents);
        order.ChangeOrderLineQuantity(Assert.Single(order.OrderLines).Id, 3);
        Assert.Equal(300m, order.TotalAmount);
        Assert.IsType<OrderLineQuantityChanged>(order.DomainEvents.Last());
        Assert.Equal(2, order.DomainEvents.Select(e => e.EventId).Distinct().Count());
        order.ClearDomainEvents();
        Assert.Empty(order.DomainEvents);
        Assert.Throws<InvalidOperationException>(() => order.AddOrderLine(product, 1));
    }

    /// <summary>扣除庫存不能產生負數。</summary>
    [Fact]
    public void Inventory_rejects_overdraw_without_changing_quantity()
    {
        var inventory = new Inventory(Guid.NewGuid());
        var productId = Guid.NewGuid();
        inventory.AdjustItem(productId, 5);
        Assert.Throws<InvalidOperationException>(() => inventory.AdjustItem(productId, -6));
        Assert.Equal(5, Assert.Single(inventory.Items).Quantity);
        Assert.IsAssignableFrom<DomainEntity<Guid>>(Assert.Single(inventory.Items));
    }

    /// <summary>地址使用套件相等性與雜湊，必填欄位仍受到驗證。</summary>
    [Fact]
    public void Address_has_value_equality_and_validates_required_fields()
    {
        var first = CreateAddress();
        var second = CreateAddress();
        Assert.IsAssignableFrom<IValueObject>(first);
        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.Throws<ArgumentException>(() => new Address(" ", "City", "State", "100", "TW", "Tester", "0912345678"));
    }

    /// <summary>相同內容的兩份訂單快照不因集合實例不同而被判為不同。</summary>
    [Fact]
    public void Snapshot_equality_compares_line_contents()
    {
        var id = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var date = DateTime.UtcNow;
        OrderSnapshot Create() => new(id, "Customer", date,
            new[] { new OrderLineSnapshot(productId, "Milk", 2, 1m, "10x10x10", true) });
        var first = Create();
        var second = Create();
        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.NotEqual(first, new OrderSnapshot(id, "Other", date, first.OrderLines));
    }

    /// <summary>出貨狀態只能依既定順序轉移。</summary>
    [Fact]
    public void Shipment_requires_shipping_before_delivery()
    {
        var snapshot = new OrderSnapshot(Guid.NewGuid(), "Customer", DateTime.UtcNow,
            new[] { new OrderLineSnapshot(Guid.NewGuid(), "Milk", 2, 1m, "10x10x10", true) });
        var shipment = new Shipment(Guid.NewGuid(), snapshot, CreateAddress());
        Assert.Equal(2m, shipment.TotalWeight);
        Assert.True(shipment.RequiresRefrigeration);
        Assert.Single(shipment.Packages);
        Assert.Throws<InvalidOperationException>(() => shipment.MarkAsDelivered());
        shipment.MarkAsShipped();
        shipment.MarkAsDelivered();
        Assert.Equal(ShipmentStatus.Delivered, shipment.Status);
    }

    /// <summary>文件識別碼不可被重新指派，敘述變更必須保留事件內容。</summary>
    [Fact]
    public void Document_identity_is_stable_and_events_preserve_old_and_new_values()
    {
        var document = new SimpleDocumentEntity("DOC-1", "old");
        document.AssignIdentity(123);
        Assert.Throws<InvalidOperationException>(() => document.AssignIdentity(124));
        var domainEvent = document.UpdateDescription("new");
        Assert.Equal("old", domainEvent.OldDescription);
        Assert.Equal("new", domainEvent.NewDescription);
        Assert.Equal(123, domainEvent.SerialId);
        Assert.Same(domainEvent, Assert.Single(document.DomainEvents));
        Assert.NotEqual(Guid.Empty, domainEvent.EventId);
        Assert.Empty(SimpleDocumentEntity.Restore(123, "DOC-1", "saved").DomainEvents);
    }

    private static Product CreateProduct() => new(Guid.NewGuid(), "Milk", 100m, 1m, "10x10x10", true);
    private static Address CreateAddress() => new("Street", "City", "State", "100", "TW", "Tester", "0912345678");
}