using System;
using dotnetLab.BuildingBlocks.Domain;

namespace dotnetLab.Domain.Shipments.ValueObjects;

/// <summary>
/// 地址 Value Object
/// </summary>
public record Address : ValueObjectBase
{
    public Address(
        string street,
        string city,
        string state,
        string postalCode,
        string country,
        string contactName,
        string contactPhone)
    {
        this.Street = street;
        this.City = city;
        this.State = state;
        this.PostalCode = postalCode;
        this.Country = country;
        this.ContactName = contactName;
        this.ContactPhone = contactPhone;

        this.ValidateSelf();
    }

    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }
    public string Country { get; }
    public string ContactName { get; }
    public string ContactPhone { get; }

    // 提供格式化的完整地址
    public string FullAddress => $"{this.Street}, {this.City}, {this.State} {this.PostalCode}, {this.Country}";

    /// <summary>
    /// 驗證地址資料的有效性
    /// </summary>
    protected override void ValidateSelf()
    {
        if (IsNullOrWhitespace(this.Street))
        {
            throw new ArgumentException("街道不能為空", nameof(this.Street));
        }

        if (IsNullOrWhitespace(this.City))
        {
            throw new ArgumentException("城市不能為空", nameof(this.City));
        }

        if (IsNullOrWhitespace(this.PostalCode))
        {
            throw new ArgumentException("郵遞區號不能為空", nameof(this.PostalCode));
        }

        if (IsNullOrWhitespace(this.Country))
        {
            throw new ArgumentException("國家不能為空", nameof(this.Country));
        }

        if (IsNullOrWhitespace(this.ContactName))
        {
            throw new ArgumentException("聯絡人不能為空", nameof(this.ContactName));
        }

        if (IsNullOrWhitespace(this.ContactPhone))
        {
            throw new ArgumentException("聯絡電話不能為空", nameof(this.ContactPhone));
        }
    }
}