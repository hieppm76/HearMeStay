using HearMeStay.Services.Interfaces;

namespace HearMeStay.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public FileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadAccommodationImageAsync(IFormFile file)
        {
            return await UploadFileAsync(file, "uploads/accommodations");
        }

        public async Task<string> UploadRoomImageAsync(IFormFile file)
        {
            return await UploadFileAsync(file, "uploads/rooms");
        }

        private async Task<string> UploadFileAsync(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ.");

            if (file.Length > MaxFileSize)
                throw new ArgumentException("File vượt quá 5MB.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(ext))
                throw new ArgumentException("Chỉ hỗ trợ file jpg, jpeg, png, webp.");

            var uploadsPath = Path.Combine(_env.WebRootPath, subFolder);
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/{subFolder}/{fileName}";
        }
    }
}
