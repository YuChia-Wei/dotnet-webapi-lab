using dotnetLab.Application.SimpleDocument.Ports.Out;
using dotnetLab.Domain.SampleDoc;

namespace dotnetLab.Application.SimpleDocument.Commands;

/// <summary>
/// command handler
/// </summary>
public class InputSimpleDocumentCommandHandler
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
        var sampleData = new SimpleDocumentEntity(request.DocumentNum, request.Description);

        return await this._simpleDocumentRepository.SaveAsync(sampleData);
    }
}