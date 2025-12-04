
public partial class UserInformation : BaseEntity
{
    public bool? IsActive { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Address { get; set; }

    public string? UserType { get; set; }

    public string UserPassword { get; set; } = null!;

    public string? PhotoUrl { get; set; }
}
