using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MessAidVOne.Application.DTOs.Requests
{
    public class MessInfoRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
        public string Name { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string? Address { get; set; }

        public IFormFile? Photo { get; set; }
    }
    
    public class AddMessRequest : MessInfoRequest
    {
       
    }

    public class ModifyMessRequest : MessInfoRequest
    {
        [Required(ErrorMessage = "Id is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Id must be a positive number.")]
        public long Id { get; set; }

        public bool IsPhotoRemove { get; set; } = false;
    }

}
