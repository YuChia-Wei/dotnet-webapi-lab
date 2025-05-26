namespace dotnetLab.Domains.SampleDoc.Events;

public record SimpleDocumentDescriptionUpdatedEvent
{
    public int SerialId { get; set; }
    public string? OldDescription { get; set; }
    public string? NewDescription { get; set; }
}