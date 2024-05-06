using dotnetLab.UseCase.SimpleDocument.Commands;
using FluentValidation;

namespace dotnetLab.WebApi.Controllers.Validator;

/// <summary>
/// InputSimpleDocumentCommand 的參數驗證器
/// </summary>
public class InputSimpleDocumentCommandValidator : AbstractValidator<InputSimpleDocumentCommand>
{
    public InputSimpleDocumentCommandValidator()
    {
        this.RuleFor(o => o.DocumentNum)
            .NotEmpty();

        this.RuleFor(o => o.Description)
            .NotEmpty();
    }
}