using dotnetLab.UseCase.SimpleDocument.Queries;
using FluentValidation;

namespace dotnetLab.WebApi.Controllers.Validator;

/// <summary>
/// SimpleDocQuery 的參數驗證器
/// </summary>
public class SimpleDocQueryValidator : AbstractValidator<SimpleDocQuery>
{
    public SimpleDocQueryValidator()
    {
        this.RuleFor(o => o.SerialId)
            .NotEmpty();
    }
}