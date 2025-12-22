using System.ComponentModel.DataAnnotations;

namespace MessAidVOne.Application.Features.AuthManagement
{
    public class OtpVerificationCommand
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
}
