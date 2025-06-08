using dotnetLab.UseCases.SimpleDocument.Dtos;
using dotnetLab.UseCases.SimpleDocument.Ports.Out;

namespace dotnetLab.UseCases.SimpleDocument.Queries;

public class SimpleDataQueryHandler
{
    private readonly ISimpleDocumentRepository _simpleDocumentRepository;

    /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
    public SimpleDataQueryHandler(ISimpleDocumentRepository simpleDocumentRepository)
    {
        this._simpleDocumentRepository = simpleDocumentRepository;
    }

    /// <summary>Handles a request</summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public async ValueTask<SimpleDocumentDto> Handle(SimpleDocQuery request, CancellationToken cancellationToken)
    {
        var sampleData = await this._simpleDocumentRepository.GetAsync(request.SerialId);

        return new SimpleDocumentDto
        {
            SerialId = sampleData.SerialId,
            Description = sampleData.Description,
            DocumentNum = sampleData.DocumentNum
        };
    }
}