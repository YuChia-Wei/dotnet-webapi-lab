using dotnetLab.UseCase.SimpleDocument.Ports.Out;
using Wolverine;

namespace dotnetLab.UseCase.SimpleDocument.Commands;

/// <summary>
/// command handler
/// </summary>
public class UpdateSimpleDocumentDescriptionCommandHandler
{
    private readonly IMessageBus _messageBus;
    private readonly ISimpleDocumentRepository _simpleDocumentRepository;

    public UpdateSimpleDocumentDescriptionCommandHandler(
        ISimpleDocumentRepository simpleDocumentRepository,
        IMessageBus messageBus)
    {
        this._simpleDocumentRepository = simpleDocumentRepository;
        this._messageBus = messageBus;
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

        var isUpdateSuccess = await this._simpleDocumentRepository.UpdateAsync(simpleDocumentEntity);

        if (isUpdateSuccess)
        {
            await this._messageBus.PublishAsync(@event);
        }

        return isUpdateSuccess;
    }
}