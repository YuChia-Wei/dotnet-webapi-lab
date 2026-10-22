using dotnetLab.Domain.SampleDoc.Events;
using Microsoft.Extensions.Logging;

namespace dotnetLab.Application.SimpleDocument.Events;

/// <summary>記錄文件敘述更新的程序內通知。</summary>
public class PublishUpdatedInformation
{
    private readonly ILogger<PublishUpdatedInformation> _logger;

    /// <summary>建立使用應用程式記錄器的事件處理器。</summary>
    public PublishUpdatedInformation(ILogger<PublishUpdatedInformation> logger)
    {
        this._logger = logger;
    }

    /// <summary>記錄通知並遵守取消權杖。</summary>
    public ValueTask HandleAsync(SimpleDocumentDescriptionUpdatedEvent notification, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        this._logger.LogInformation("Document {SerialId} description updated; event {EventId}.", notification.SerialId, notification.EventId);
        return ValueTask.CompletedTask;
    }
}