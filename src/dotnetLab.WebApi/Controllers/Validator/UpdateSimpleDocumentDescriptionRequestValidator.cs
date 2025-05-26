using dotnetLab.UseCases.SimpleDocument.Commands;
using dotnetLab.WebApi.Controllers.Requests;
using FluentValidation;

namespace dotnetLab.WebApi.Controllers.Validator;

/// <summary>
/// UpdateSimpleDocumentDescriptionCommand 的參數驗證器
/// </summary>
public class UpdateSimpleDocumentDescriptionRequestValidator : AbstractValidator<UpdateSimpleDocumentDescriptionRequest>
{
    public UpdateSimpleDocumentDescriptionRequestValidator()
    {
        this.RuleFor(o => o.Description)
            .NotEmpty();
    }
}