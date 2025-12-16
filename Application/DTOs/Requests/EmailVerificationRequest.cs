using System.ComponentModel.DataAnnotations;

namespace MessAidVOne.Application.DTOs.Requests
{
    public class EmailVerificationRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
        public string Email { get; set; } = null!;
    }
}
