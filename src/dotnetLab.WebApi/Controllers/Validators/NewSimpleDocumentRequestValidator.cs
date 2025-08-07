using dotnetLab.WebApi.Controllers.Requests;
using FluentValidation;

namespace dotnetLab.WebApi.Controllers.Validator;

/// <summary>
/// InputSimpleDocumentCommand 的參數驗證器
/// </summary>
public class NewSimpleDocumentRequestValidator : AbstractValidator<NewSimpleDocumentRequest>
{
    /// <inheritdoc />
    public NewSimpleDocumentRequestValidator()
    {
        this.RuleFor(o => o.DocumentNum)
            .NotEmpty();

        this.RuleFor(o => o.Description)
            .NotEmpty();
    }
}