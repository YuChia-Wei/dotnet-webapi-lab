using FluentValidation;

namespace dotnetLab.WebApi.Infrastructure.Attributes;

/// <summary>
/// 參數驗證器 (泛型版本，可利用泛型型別限制避免誤用)
/// </summary>
/// <typeparam name="TValidator"></typeparam>
public class ParameterValidatorAttribute<TValidator> : ParameterValidatorAttribute
    where TValidator : IValidator
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
        : base(typeof(TValidator), parameterName)
    {
    }
}