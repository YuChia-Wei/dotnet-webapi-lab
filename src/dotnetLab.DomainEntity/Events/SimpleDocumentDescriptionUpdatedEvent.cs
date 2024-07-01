using Mediator;

namespace dotnetLab.DomainEntity.Events;

public record SimpleDocumentDescriptionUpdatedEvent : INotification
{
    public int SerialId { get; set; }
    public string? OldDescription { get; set; }
    public string? NewDescription { get; set; }
}