using ArchPrecept.Contracts.Domain;

namespace dotnetLab.Domain.SampleDoc.Events;

/// <summary>記錄文件敘述修改前後的內容。</summary>
public sealed record SimpleDocumentDescriptionUpdatedEvent : IDomainEvent
{
    /// <inheritdoc />
    public Guid EventId { get; } = Guid.NewGuid();
    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    /// <summary>文件序號。</summary>
    public int SerialId { get; init; }
    /// <summary>修改前的敘述。</summary>
    public string? OldDescription { get; init; }
    /// <summary>修改後的敘述。</summary>
    public string? NewDescription { get; init; }
}