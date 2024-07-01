using dotnetLab.UseCase.SimpleDocument.Commands;
using FluentValidation;

namespace dotnetLab.WebApi.Controllers.Validator;

/// <summary>
/// UpdateSimpleDocumentDescriptionCommand 的參數驗證器
/// </summary>
public class UpdateSimpleDocumentDescriptionCommandValidator : AbstractValidator<UpdateSimpleDocumentDescriptionCommand>
{
    public UpdateSimpleDocumentDescriptionCommandValidator()
    {
        this.RuleFor(o => o.Description)
            .NotEmpty();
    }
}