namespace dotnetLab.WebApi.Infrastructure.Attributes;

/// <summary>
/// 參數驗證器 Attribute
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute" />
public class ParameterValidatorAttribute<T> : ParameterValidatorAttribute
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see>
    ///     <cref>ParameterValidatorAttribute</cref>
    /// </see>
    /// class.
    /// </summary>
    /// <param name="parameterName">參數名稱</param>
    public ParameterValidatorAttribute(string? parameterName = null)
        : base(typeof(T), parameterName)
    {
    }
}