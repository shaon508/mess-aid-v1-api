public partial class ActivityInformation : BaseEntity
{
    public long ActorUserId { get; set; }

    public long EntityId { get; set; }

    public string EntityType { get; set; } = null!;

    public string EventKey { get; set; } = null!;

    public virtual ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
}
