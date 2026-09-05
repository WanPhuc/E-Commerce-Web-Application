using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Outbox;

public class OutboxMessage
{
    [Key]
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime OccurredOnUtc { get; set; }
    public DateTime? ProcessedOnUtc { get; set; }
}
