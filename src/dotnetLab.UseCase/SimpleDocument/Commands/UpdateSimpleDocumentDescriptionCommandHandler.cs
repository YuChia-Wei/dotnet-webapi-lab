using dotnetLab.UseCase.SimpleDocument.Ports.Out;

namespace dotnetLab.UseCase.SimpleDocument.Commands;

/// <summary>
/// command handler
/// </summary>
public class UpdateSimpleDocumentDescriptionCommandHandler : IRequestHandler<UpdateSimpleDocumentDescriptionCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ISimpleDocumentRepository _simpleDocumentRepository;

    public UpdateSimpleDocumentDescriptionCommandHandler(
        ISimpleDocumentRepository simpleDocumentRepository,
        IMediator mediator)
    {
        this._simpleDocumentRepository = simpleDocumentRepository;
        this._mediator = mediator;
    }

    /// <summary>Handles a request</summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public async ValueTask<bool> Handle(UpdateSimpleDocumentDescriptionCommand request, CancellationToken cancellationToken)
    {
        var simpleDocumentEntity = await this._simpleDocumentRepository.GetAsync(request.SerialId);

        if (simpleDocumentEntity is null)
        {
            return false;
        }

        var @event = simpleDocumentEntity.UpdateDescription(request.Description);

        await this._mediator.Publish(@event, cancellationToken);

        return await this._simpleDocumentRepository.UpdateAsync(simpleDocumentEntity);
    }
}