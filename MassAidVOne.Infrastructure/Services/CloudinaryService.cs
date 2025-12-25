using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using MassAidVOne.Infrastructure.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MassAidVOne.Application.Interfaces;

namespace MassAidVOne.Infrastructure.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> settings)
        {
            var account = new Account(
                settings.Value.CloudName,
                settings.Value.ApiKey,
                settings.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<(bool IsSuccess, string Message, string? Data)> UploadImageAsync(IFormFile file)
        {
            var (IsSuccess, Message) = ValidateFile(file);
            if (!IsSuccess)
            {
                return (false, Message, null);
            }

            try
            {
                using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "mess-management",
                    Transformation = new Transformation()
                        .Quality("auto")
                        .FetchFormat("auto")
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                return (true, "OK", result.SecureUrl.ToString());
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }

        }

        private (bool IsSuccess, string Message) ValidateFile(IFormFile file)
        {

            if (file == null || file.Length == 0)
                return (false, "Invalid file");

            if (file.Length > 5 * 1024 * 1024)
                return (false, "Max file size is 5MB");

            var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return (false, $"Invalid file type. Allowed types: {string.Join(", ", allowedExtensions)}");
            }

            return (true, string.Empty);
        }

    }
}
