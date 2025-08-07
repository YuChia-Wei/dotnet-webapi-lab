using System;
using System.Collections.Generic;
using dotnetLab.BuildingBlocks.Domain;

namespace dotnetLab.Domain.Shipments.Entities;

/// <summary>
/// 貨運包裹實體
/// </summary>
public class ShipmentPackage : IDomainEntity<Guid>
{
    private readonly List<IDomainEvent> _domainEvents = new();

    // 供 EF Core 等 ORM 使用的私有建構函式
    private ShipmentPackage() { }

    /// <summary>
    /// 建立貨運包裹
    /// </summary>
    internal ShipmentPackage(
        Guid id,
        Guid shipmentId,
        string description,
        decimal weight,
        bool isRefrigerated)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("包裹ID不能為空", nameof(id));
        }

        if (shipmentId == Guid.Empty)
        {
            throw new ArgumentException("貨運ID不能為空", nameof(shipmentId));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("描述不能為空", nameof(description));
        }

        if (weight <= 0)
        {
            throw new ArgumentException("重量必須大於零", nameof(weight));
        }

        this.Id = id;
        this.ShipmentId = shipmentId;
        this.Description = description;
        this.Weight = weight;
        this.IsRefrigerated = isRefrigerated;
        this.ScanCode = this.GenerateScanCode();
    }

    public Guid ShipmentId { get; private set; }
    public string Description { get; private set; }
    public decimal Weight { get; private set; }
    public bool IsRefrigerated { get; private set; }
    public string ScanCode { get; private set; }

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
    /// 更新包裹重量
    /// </summary>
    internal void UpdateWeight(decimal newWeight)
    {
        if (newWeight <= 0)
        {
            throw new ArgumentException("重量必須大於零", nameof(newWeight));
        }

        this.Weight = newWeight;
    }

    /// <summary>
    /// 生成掃描碼
    /// </summary>
    private string GenerateScanCode()
    {
        // 簡化的掃描碼生成邏輯
        return $"PKG-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
    }
}