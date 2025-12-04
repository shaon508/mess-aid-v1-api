using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

public class UserInformationDto
{
    public long Id { get; set; }

    public bool IsActive { get; set; } = true;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Address { get; set; }

    public string? UserType { get; set; }

    public string? PhotoUrl { get; set; }
}
public class AddUserRequest : UserInfoRequest
{
    [Required(ErrorMessage = "Otp id is required.")]
    [Range(1, long.MaxValue, ErrorMessage = "Otp id must be a positive number.")]
    public long OtpId { get; set; }

    [Required(ErrorMessage = "Otp code is required.")]
    [Range(1000, 9999, ErrorMessage = "Otp code must be a 4-digit number.")]
    public long OtpCode { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(50, ErrorMessage = "Password cannot exceed 50 characters.")]
    public string Password { get; set; } = null!;
}

public class ModifyUserRequest : UserInfoRequest
{
    [Required(ErrorMessage = "Id is required.")]
    [Range(1, long.MaxValue, ErrorMessage = "Id must be a positive number.")]
    public long Id { get; set; }
}

public class UserInfoRequest
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
    public string Email { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
    public string? Address { get; set; }
    public IFormFile? Photo { get; set; }
}

