using dotnetLab.DomainEntity.Events;
using Microsoft.Extensions.Logging;

namespace dotnetLab.UseCase.SimpleDocument.Events;

public class SimpleDocDescUpdatedEventHandler : INotificationHandler<SimpleDocumentDescriptionUpdatedEvent>
{
    private readonly ILogger<SimpleDocDescUpdatedEventHandler> _logger;

    public SimpleDocDescUpdatedEventHandler(ILogger<SimpleDocDescUpdatedEventHandler> logger)
    {
        this._logger = logger;
    }

    public ValueTask Handle(SimpleDocumentDescriptionUpdatedEvent notification, CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{DateTime.Now} Simple Document Description was updated!");
        return ValueTask.CompletedTask;
    }
}