using System.ComponentModel.DataAnnotations;

namespace MessAidVOne.Application.DTOs.Requests
{
    public class ForgetPasswordRequest : OtpVerificationRequest
    {
        [Required(ErrorMessage = "New password is required.")]
        [StringLength(50, ErrorMessage = "New password cannot exceed 50 characters.")]
        public string NewPassword { get; set; } = null!;
    }
}
