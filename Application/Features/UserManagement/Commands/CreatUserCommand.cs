using System.ComponentModel.DataAnnotations;
using MessAidVOne.Application.Abstructions;
using MessAidVOne.Application.Features.AuthManagement.Commands;
using Microsoft.AspNetCore.Http;

namespace MessAidVOne.Application.Features.UserManagement.Commands
{
    public class CreatUserCommand : OtpVerificationCommand, ICommand<Result<UserInformationDto>>
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

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, ErrorMessage = "Password cannot exceed 50 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must contain at least 8 characters, one uppercase, one lowercase, one number and one special character")]
        public string Password { get; set; } = null!;
    }
}
