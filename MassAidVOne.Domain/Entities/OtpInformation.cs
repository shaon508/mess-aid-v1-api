

public partial class OtpInformation : BaseEntity
{
    public string Email { get; set; } = null!;

    public int OtpCode { get; set; }

    public string Status { get; set; } = null!;

    public string ActivityType { get; set; } = null!;

    public int? Attempts { get; set; }
    public int? LifeTime { get; set; }
}
