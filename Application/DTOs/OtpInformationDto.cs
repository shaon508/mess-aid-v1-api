
using System.ComponentModel.DataAnnotations;

public class OtpInformationDto
{
    public long OtpId { get; set; }
    public string Email { get; set; } = null!;

}
public class EmailVerificationRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
    public string Email { get; set; } = null!;
}

public class OtpVerificationRequest
{
    [Required(ErrorMessage = "Otp id is required.")]
    [Range(1, long.MaxValue, ErrorMessage = "Otp id must be a positive number.")]
    public long OtpId { get; set; }

    [Required(ErrorMessage = "Otp code is required.")]
    [Range(1000, 9999, ErrorMessage = "Otp code must be a 4-digit number.")]
    public int OtpCode { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
    public string Email { get; set; } = null!;

}

public class ForgetPasswordRequest : OtpVerificationRequest
{
    [Required(ErrorMessage = "New password is required.")]
    [StringLength(50, ErrorMessage = "New password cannot exceed 50 characters.")]
    public string NewPassword { get; set; } = null!;
}

public class LogInRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(50, ErrorMessage = "Password cannot exceed 50 characters.")]
    public string Password { get; set; } = null!;

}

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Old password is required.")]
    [StringLength(50, ErrorMessage = "Old password cannot exceed 50 characters.")]
    public string OldPassword { get; set; } = null!;

    [Required(ErrorMessage = "New password is required.")]
    [StringLength(50, ErrorMessage = "New password cannot exceed 50 characters.")]
    public string NewPassword { get; set; } = null!;

}


