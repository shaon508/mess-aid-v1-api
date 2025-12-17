namespace MassAidVOne.Domain.Entities;

public partial class MemberInformation : BaseEntity
{
    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? PhotoUrl { get; set; }

    public string Type { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? JoinedDate { get; set; }

    public DateTime? LeaveDate { get; set; }

    public string? UserId { get; set; }

    public bool? IsMealAutoUpdate { get; set; }

    public long MessId { get; set; }

    public virtual MessInformation Mess { get; set; } = null!;
}
