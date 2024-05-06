using dotnetLab.DomainEntity;
using dotnetLab.UseCase.SimpleDocument.Ports.Out;

namespace dotnetLab.UseCase.SimpleDocument.Commands;

/// <summary>
/// command handler
/// </summary>
public class InputSimpleDocumentCommandHandler : IRequestHandler<InputSimpleDocumentCommand, int>
{
    private readonly ISimpleDocumentRepository _simpleDocumentRepository;

    public InputSimpleDocumentCommandHandler(ISimpleDocumentRepository simpleDocumentRepository)
    {
        this._simpleDocumentRepository = simpleDocumentRepository;
    }

    /// <summary>Handles a request</summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public async ValueTask<int> Handle(InputSimpleDocumentCommand request, CancellationToken cancellationToken)
    {
        var sampleData = new SimpleDocumentEntity(request.DocumentNum, request.Description) ;

        return await this._simpleDocumentRepository.SaveAsync(sampleData);
    }
}