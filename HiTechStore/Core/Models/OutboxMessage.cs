namespace HiTechStore.Core.Models;


public class OutboxMessage : IModel
{
    public Guid Id { get; set; }

    required public string EventType { get; set; }

    public string? Payload { get; set; }

    public DateTime OccurredAt { get; set; }

    public DateTime? ProcessedAt { get; set; }
    public int RetryCount { get; set; } = 0;
    public string? LastError { get; set; }
    public DateTime LastAttemptAt { get; set; }
}