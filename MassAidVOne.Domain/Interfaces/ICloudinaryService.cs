using Microsoft.AspNetCore.Http;

namespace MassAidVOne.Application.Interfaces
{
    public interface ICloudinaryService
    {
       Task<(bool IsSuccess, string Message, string? Data)> UploadImageAsync(IFormFile file);
    }
}
