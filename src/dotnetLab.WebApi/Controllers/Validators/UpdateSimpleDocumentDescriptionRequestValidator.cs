using dotnetLab.WebApi.Controllers.Requests;
using FluentValidation;

namespace dotnetLab.WebApi.Controllers.Validator;

/// <summary>
/// UpdateSimpleDocumentDescriptionCommand 的參數驗證器
/// </summary>
public class UpdateSimpleDocumentDescriptionRequestValidator : AbstractValidator<UpdateSimpleDocumentDescriptionRequest>
{
    /// <inheritdoc />
    public UpdateSimpleDocumentDescriptionRequestValidator()
    {
        this.RuleFor(o => o.Description)
            .NotEmpty();
    }
}