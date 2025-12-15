using Microsoft.AspNetCore.Http;

namespace MassAidVOne.Infrastructure.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
