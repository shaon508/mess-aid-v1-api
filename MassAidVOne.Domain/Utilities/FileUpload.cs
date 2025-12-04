using Microsoft.AspNetCore.Http;

namespace MassAidVOne.Domain.Utilities
{
    public static class ImageUploadUtilities
    {
        public static string FileLocation;
        public static string FileUrl;
        public static int MaxSize;

        public static void Initialize(FileSetting appSettings)
        {
            FileLocation = appSettings.FileLocation;
            FileUrl = appSettings.FileUrl;
            MaxSize = appSettings.MaxSize;
        }

        public static async Task<(bool IsSuccess, string Message, string? Data)> UploadImageAsync(IFormFile file, string folderName, string fileName = "")
        {
            var (IsSuccess, Message) = await ValidateFileAsync(file);
            if (!IsSuccess)
            {
                return (false, Message, null);
            }
            try
            {
                var originalFileName = file.FileName;
                var fileExtension = Path.GetExtension(originalFileName);

                fileName = string.IsNullOrWhiteSpace(fileName)
                    ? $"{Guid.NewGuid()}{fileExtension}"
                    : $"{fileName}{fileExtension}";

                var folderPath = Path.Combine(FileLocation, folderName);
                Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return (true, "OK", $"{folderName}/{fileName}");


            }
            catch (Exception ex)
            {
                return (false, $"File upload failed: {ex.Message}", null);
            }

        }

        public static string ToLongUrl(this string url)
        {
            return $"{url}";
        }

        private static async Task<(bool IsSuccess, string Message)> ValidateFileAsync(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var maxFileSize = MaxSize * 1024 * 1024; 
            if (file == null || file.Length == 0)
            {
                return (false, "No file provided.");
            }
            if (file.Length > MaxSize)
            {
                return (false, $"File size exceeds the maximum limit of {MaxSize} bytes.");
            }
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return (false, "Invalid file type. Only image files are allowed.");
            }
            return (true, string.Empty);
        }

    }
}
