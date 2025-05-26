using System.Text.Json;

namespace dotnetLab.Infrastructure.ValueObjects;

/// <summary>
/// 值物件基底類別
/// 提供常用的值物件共用功能
/// </summary>
public abstract record ValueObjectBase : IValueObject
{
    /// <summary>
    /// 產生值物件的 JSON 字串表示
    /// </summary>
    public override string ToString() => JsonSerializer.Serialize(this, GetType());

    /// <summary>
    /// 檢查值物件的屬性是否有效
    /// 子類應覆寫此方法以實作特定的驗證邏輯
    /// </summary>
    /// <exception cref="InvalidOperationException">當值物件包含無效資料時拋出</exception>
    protected virtual void ValidateSelf()
    {
        // 基底方法不做任何驗證
    }

    /// <summary>
    /// 檢查字串是否為空白或 null
    /// </summary>
    protected static bool IsNullOrWhitespace(string? value) => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// 檢查數值是否小於等於零
    /// </summary>
    protected static bool IsLessThanOrEqualToZero(decimal value) => value <= 0;

    /// <summary>
    /// 檢查數值是否小於等於零
    /// </summary>
    protected static bool IsLessThanOrEqualToZero(int value) => value <= 0;

    /// <summary>
    /// 檢查 GUID 是否為空
    /// </summary>
    protected static bool IsEmpty(Guid value) => value == Guid.Empty;
}
