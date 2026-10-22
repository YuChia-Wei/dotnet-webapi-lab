using ArchPrecept.Contracts.Domain;

namespace dotnetLab.Domain.Shipments.ValueObjects;

/// <summary>
/// 地址 Value Object
/// </summary>
public sealed class Address : ValueObject
{
    /// <summary>建立並驗證Address值物件。</summary>
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
        this.State = state ?? string.Empty;
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
    private void ValidateSelf()
    {
        if (string.IsNullOrWhiteSpace(this.Street))
        {
            throw new ArgumentException("街道不能為空", nameof(this.Street));
        }

        if (string.IsNullOrWhiteSpace(this.City))
        {
            throw new ArgumentException("城市不能為空", nameof(this.City));
        }

        if (string.IsNullOrWhiteSpace(this.PostalCode))
        {
            throw new ArgumentException("郵遞區號不能為空", nameof(this.PostalCode));
        }

        if (string.IsNullOrWhiteSpace(this.Country))
        {
            throw new ArgumentException("國家不能為空", nameof(this.Country));
        }

        if (string.IsNullOrWhiteSpace(this.ContactName))
        {
            throw new ArgumentException("聯絡人不能為空", nameof(this.ContactName));
        }

        if (string.IsNullOrWhiteSpace(this.ContactPhone))
        {
            throw new ArgumentException("聯絡電話不能為空", nameof(this.ContactPhone));
        }
    }

    /// <inheritdoc />
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Street;
        yield return this.City;
        yield return this.State;
        yield return this.PostalCode;
        yield return this.Country;
        yield return this.ContactName;
        yield return this.ContactPhone;
    }
}