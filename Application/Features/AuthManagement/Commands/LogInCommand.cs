using System.ComponentModel.DataAnnotations;
using MessAidVOne.Application.Abstructions;
using MessAidVOne.Application.DTOs;

namespace MessAidVOne.Application.Features.AuthManagement.Commands
{
    public class LogInCommand : ICommand<Result<LoginDto>>
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, ErrorMessage = "Password cannot exceed 50 characters.")]
        public string Password { get; set; } = null!;
    }
}
