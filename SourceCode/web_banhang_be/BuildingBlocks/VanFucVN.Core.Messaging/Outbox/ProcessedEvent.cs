using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Outbox;

public class ProcessedEvent
{
    [Key]
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTime ProcessedOnUtc { get; set; }
}
