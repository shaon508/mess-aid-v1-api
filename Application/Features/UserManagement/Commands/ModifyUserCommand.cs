using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MessAidVOne.Application.Abstructions;
using Microsoft.AspNetCore.Http;

namespace MessAidVOne.Application.Features.UserManagement.Commands
{
    public class ModifyUserCommand : ICommand<Result<UserInformationDto>>
    {
        [Required(ErrorMessage = "Id is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Id must be a positive number.")]
        public long Id { get; set; }

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
        public bool IsPhotoRemove { get; set; } = false;

    }
}
