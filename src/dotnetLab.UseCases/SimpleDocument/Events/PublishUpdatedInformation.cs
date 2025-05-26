using dotnetLab.Domains.SampleDoc.Events;
using Microsoft.Extensions.Logging;

namespace dotnetLab.UseCases.SimpleDocument.Events;

public class PublishUpdatedInformation
{
    private readonly ILogger<PublishUpdatedInformation> _logger;

    public PublishUpdatedInformation(ILogger<PublishUpdatedInformation> logger)
    {
        this._logger = logger;
    }

    public ValueTask Handle(SimpleDocumentDescriptionUpdatedEvent notification, CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{DateTime.Now} Simple Document Description was updated!");
        return ValueTask.CompletedTask;
    }
}