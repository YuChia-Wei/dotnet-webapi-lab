namespace dotnetLab.DomainEntity.Events;

public record SimpleDocumentDescriptionUpdatedEvent : IDomainEvent
{
    public int SerialId { get; set; }
    public string? OldDescription { get; set; }
    public string? NewDescription { get; set; }
}