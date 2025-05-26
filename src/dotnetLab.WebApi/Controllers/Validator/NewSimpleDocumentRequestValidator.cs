using dotnetLab.UseCases.SimpleDocument.Commands;
using dotnetLab.WebApi.Controllers.Requests;
using FluentValidation;

namespace dotnetLab.WebApi.Controllers.Validator;

/// <summary>
/// InputSimpleDocumentCommand 的參數驗證器
/// </summary>
public class NewSimpleDocumentRequestValidator : AbstractValidator<NewSimpleDocumentRequest>
{
    public NewSimpleDocumentRequestValidator()
    {
        this.RuleFor(o => o.DocumentNum)
            .NotEmpty();

        this.RuleFor(o => o.Description)
            .NotEmpty();
    }
}