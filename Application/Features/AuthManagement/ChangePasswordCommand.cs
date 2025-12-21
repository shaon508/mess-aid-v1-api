using System.ComponentModel.DataAnnotations;
using MessAidVOne.Application.Abstructions;

namespace MessAidVOne.Application.Features.AuthManagement
{
    public class ChangePasswordCommand : ICommand<Result<bool>>
    {
        [Required(ErrorMessage = "Old password is required.")]
        [StringLength(50, ErrorMessage = "Old password cannot exceed 50 characters.")]
        public string OldPassword { get; set; } = null!;

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(50, ErrorMessage = "New password cannot exceed 50 characters.")]
        public string NewPassword { get; set; } = null!;
    }
}
