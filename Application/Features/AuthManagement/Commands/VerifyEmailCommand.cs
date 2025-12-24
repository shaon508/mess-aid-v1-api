using System.ComponentModel.DataAnnotations;
using MessAidVOne.Application.Abstructions;

namespace MessAidVOne.Application.Features.AuthManagement.Commands
{
    public class VerifyEmailCommand : ICommand<Result<OtpInformationDto>>
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
        public string Email { get; set; } = null!;
    }
}
