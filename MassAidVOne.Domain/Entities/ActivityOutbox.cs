public partial class ActivityOutbox : BaseEntity
{
    public string EventKey { get; set; } = null!;

    public sbyte Domain { get; set; }

    public long ActorUserId { get; set; }

    public long EntityId { get; set; }

    public string EntityType { get; set; } = null!;

    public string PayloadJson { get; set; } = null!;

    public sbyte Status { get; set; }

    public int ProcessingAttempts { get; set; }

    public DateTime? ProcessedAt { get; set; }
}
