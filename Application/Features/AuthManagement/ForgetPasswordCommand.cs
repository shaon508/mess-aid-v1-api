using System.ComponentModel.DataAnnotations;
using MessAidVOne.Application.Abstructions;

namespace MessAidVOne.Application.Features.AuthManagement
{
    public class ForgetPasswordCommand : OtpVerificationCommand, ICommand<Result<bool>>
    {
        [Required(ErrorMessage = "New password is required.")]
        [StringLength(50, ErrorMessage = "New password cannot exceed 50 characters.")]
        public string NewPassword { get; set; } = null!;
    }
}
