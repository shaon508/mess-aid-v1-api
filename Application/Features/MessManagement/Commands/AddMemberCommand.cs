using System.ComponentModel.DataAnnotations;
using MessAidVOne.Application.Abstructions;
using Microsoft.AspNetCore.Http;

namespace MessAidVOne.Application.Features.MessManagement.Commands
{
    public class AddMemberCommand : ICommand<Result<bool>>
    {

        [Required(ErrorMessage = "Mess id is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Mess id must be a positive number.")]
        public long MessId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
        public string Email { get; set; } = null!;

        public IFormFile? Photo { get; set; }

        public bool? IsMealAutoUpdate { get; set; } = false;
        
    }
}
