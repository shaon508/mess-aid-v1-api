public class UserInformationResponseDto
{
    public long Id { get; set; }

    public bool IsActive { get; set; } = true;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Address { get; set; }

    public string? UserType { get; set; }

    public string? PhotoUrl { get; set; }
}


