namespace dotnetLab.WebApi.Infrastructure.Attributes;

/// <summary>
/// 自訂 api 權限驗證
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomApiPermissionValidatorAttribute : Attribute
{
    /// <summary>
    /// </summary>
    /// <param name="rule"></param>
    public CustomApiPermissionValidatorAttribute(string rule)
    {
        this.PermissionRule = rule;
    }

    /// <summary>
    /// </summary>
    public string PermissionRule { get; set; }
}