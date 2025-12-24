using System.ComponentModel.DataAnnotations;
using MessAidVOne.Application.Abstructions;
using Microsoft.AspNetCore.Http;

namespace MessAidVOne.Application.Features.MessManagement.Commands
{
    public class CreatMessCommand : ICommand<Result<MessInformationDto>>
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
        public string Name { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string? Address { get; set; }

        public IFormFile? Photo { get; set; }
    }
}
