namespace MassAidVOne.Domain.Entities;

public partial class MessInformation : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public bool? IsActive { get; set; }

    public string? PhotoUrl { get; set; }

    public DateTime? DeactivatedDate { get; set; }

    public virtual ICollection<MemberInformation> MemberInformations { get; set; } = new List<MemberInformation>();
}
