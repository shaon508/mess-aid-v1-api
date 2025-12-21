using Microsoft.AspNetCore.Http;

namespace MassAidVOne.Application.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
