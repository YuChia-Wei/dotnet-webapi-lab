using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dotnetLab.WebApi.Infrastructure.ParameterValidators;

/// <summary>
/// 參數驗證器 Attribute
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute" />
public class ParameterValidatorAttribute : ActionFilterAttribute
{
    private readonly string? _parameterName;
    private readonly Type _validatorType;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterValidatorAttribute" /> class.
    /// </summary>
    /// <param name="validatorType">Type of the validator.</param>
    /// <param name="parameterName">參數名稱</param>
    public ParameterValidatorAttribute(Type validatorType, string? parameterName = null)
    {
        this._validatorType = validatorType;
        this._parameterName = parameterName;
    }

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <inheritdoc />
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var parameters = context.ActionArguments;
        if (parameters.Count <= 0)
        {
            await base.OnActionExecutionAsync(context, next);
        }

        var value = string.IsNullOrEmpty(this._parameterName)
                        ? parameters.FirstOrDefault().Value
                        : parameters.FirstOrDefault(o => o.Key.Equals(this._parameterName)).Value;

        if (value == null)
        {
            context.Result = new BadRequestObjectResult("未輸入 Parameter");
        }
        else
        {
            var validationContext = new ValidationContext<object>(value);

            var validator = Activator.CreateInstance(this._validatorType) as IValidator;
            var validationResult = await validator!.ValidateAsync(validationContext).ConfigureAwait(false);

            if (validationResult.IsValid.Equals(false))
            {
                context.Result = new BadRequestObjectResult(validationResult.Errors);
            }
        }

        await base.OnActionExecutionAsync(context, next).ConfigureAwait(false);
    }
}