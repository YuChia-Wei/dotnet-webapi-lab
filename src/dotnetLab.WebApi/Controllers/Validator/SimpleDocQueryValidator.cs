using dotnetLab.WebApi.Controllers.Requests;
using FluentValidation;

namespace dotnetLab.WebApi.Controllers.Validator;

/// <summary>
/// SimpleDocQuery 的參數驗證器
/// </summary>
public class SimpleDocQueryValidator : AbstractValidator<SimpleDocQueryRequest>
{
    /// <inheritdoc />
    public SimpleDocQueryValidator()
    {
        this.RuleFor(o => o.SerialId)
            .NotEmpty();
    }
}