public partial class UserActivity : BaseEntity
{
    public long ActivityId { get; set; }

    public long UserId { get; set; }

    public bool IsRead { get; set; }

    public string? Description { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual ActivityInformation Activity { get; set; } = null!;
}
