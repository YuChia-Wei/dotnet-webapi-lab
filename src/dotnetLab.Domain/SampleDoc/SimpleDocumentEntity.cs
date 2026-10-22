using ArchPrecept.Contracts.Domain;
using dotnetLab.Domain.SampleDoc.Events;

namespace dotnetLab.Domain.SampleDoc;

/// <summary>以文件編號與敘述為狀態的文件聚合根。</summary>
public sealed class SimpleDocumentEntity : AggregateRoot<int>
{
    /// <summary>建立尚未取得儲存識別碼的文件。</summary>
    public SimpleDocumentEntity(string documentNum, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentNum);
        this.DocumentNum = documentNum;
        this.Description = description;
        this.Version = 1;
    }

    /// <summary>取得對外使用的文件序號。</summary>
    public int SerialId => this.Id;
    /// <summary>取得文件敘述。</summary>
    public string? Description { get; private set; }
    /// <summary>取得文件編號。</summary>
    public string DocumentNum { get; }

    /// <summary>將儲存層產生的正整數識別碼指派給新文件。</summary>
    public void AssignIdentity(int id)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (this.Id != 0 && this.Id != id) throw new InvalidOperationException("文件識別碼不可變更。");
        this.Id = id;
    }

    /// <summary>從儲存狀態重建文件，不產生新的領域事件。</summary>
    public static SimpleDocumentEntity Restore(int id, string documentNum, string? description)
    {
        var document = new SimpleDocumentEntity(documentNum, description);
        document.AssignIdentity(id);
        return document;
    }

    /// <summary>更新敘述，並將變更加入待分派的領域事件。</summary>
    public SimpleDocumentDescriptionUpdatedEvent UpdateDescription(string? description)
    {
        var domainEvent = new SimpleDocumentDescriptionUpdatedEvent
        {
            SerialId = this.SerialId,
            OldDescription = this.Description,
            NewDescription = description
        };
        this.Description = description;
        this.AddDomainEvent(domainEvent);
        return domainEvent;
    }
}