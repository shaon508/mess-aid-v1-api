using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MessAidVOne.Application.DTOs.Requests
{
    public class UploadPhotoRequest
    {
        public IFormFile File { get; set; } = null!;
    }

}
