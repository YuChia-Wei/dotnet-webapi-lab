using System;
using System.Collections.Generic;
using dotnetLab.BuildingBlocks.Domain;

namespace dotnetLab.Domain.Products;

/// <summary>
/// 商品聚合根
/// </summary>
public class Product : IAggregateRoot<Guid>
{
    private readonly List<IDomainEvent> _domainEvents = new();

    private Product() { }

    /// <summary>
    /// 建立商品
    /// </summary>
    /// <param name="id">商品識別碼</param>
    /// <param name="name">名稱</param>
    /// <param name="unitPrice">單價</param>
    /// <param name="weight">重量</param>
    /// <param name="dimensions">尺寸</param>
    /// <param name="requiresRefrigeration">是否需要冷藏</param>
    public Product(Guid id, string name, decimal unitPrice, decimal weight, string dimensions, bool requiresRefrigeration)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("商品ID不能為空", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("商品名稱不能為空", nameof(name));
        }

        if (unitPrice <= 0)
        {
            throw new ArgumentException("單價必須大於零", nameof(unitPrice));
        }

        if (weight <= 0)
        {
            throw new ArgumentException("重量必須大於零", nameof(weight));
        }

        if (string.IsNullOrWhiteSpace(dimensions))
        {
            throw new ArgumentException("尺寸不能為空", nameof(dimensions));
        }

        this.Id = id;
        this.Name = name;
        this.UnitPrice = unitPrice;
        this.Weight = weight;
        this.Dimensions = dimensions;
        this.RequiresRefrigeration = requiresRefrigeration;
        this.Version = 1;
    }

    /// <summary>
    /// 名稱
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// 重量
    /// </summary>
    public decimal Weight { get; private set; }

    /// <summary>
    /// 尺寸
    /// </summary>
    public string Dimensions { get; private set; }

    /// <summary>
    /// 是否需要冷藏
    /// </summary>
    public bool RequiresRefrigeration { get; private set; }

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
    /// 更新單價
    /// </summary>
    /// <param name="newPrice"></param>
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
        {
            throw new ArgumentException("單價必須大於零", nameof(newPrice));
        }

        this.UnitPrice = newPrice;
    }
}