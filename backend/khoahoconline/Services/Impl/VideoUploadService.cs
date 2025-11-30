using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using khoahoconline.Middleware.Exceptions;

namespace khoahoconline.Services.Impl
{
    public class VideoUploadService : IVideoUploadService
    {
        private readonly ILogger<VideoUploadService> _logger;
        private readonly IWebHostEnvironment _environment;

        public VideoUploadService(
            ILogger<VideoUploadService> logger,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async Task<string> UploadVideoAsync(IFormFile file, string folder = "videos")
        {
            if (file == null || file.Length == 0)
            {
                throw new BadRequestException("File video không hợp lệ");
            }

            // Validate file extension
            var allowedExtensions = new[] { ".mp4", ".webm", ".mov", ".avi" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
            {
                throw new BadRequestException("Chỉ chấp nhận file video (.mp4, .webm, .mov, .avi)");
            }

            // Validate file size (500MB max for test environment)
            var maxSize = 500 * 1024 * 1024; // 500MB
            if (file.Length > maxSize)
            {
                throw new BadRequestException("Kích thước file không được vượt quá 500MB");
            }

            // Generate unique filename
            var fileName = $"video_{Guid.NewGuid()}{extension}";
            var uploadPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads", folder);
            
            // Create directory if not exists
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation($"Video uploaded: {fileName}, Size: {file.Length} bytes");

            // Return URL path
            return $"/uploads/{folder}/{fileName}";
        }

        public async Task<bool> DeleteVideoAsync(string videoUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(videoUrl))
                {
                    return false;
                }

                // Remove leading slash if exists
                var relativePath = videoUrl.TrimStart('/');
                var filePath = Path.Combine(_environment.WebRootPath ?? "wwwroot", relativePath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation($"Video deleted: {videoUrl}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting video: {videoUrl}");
                return false;
            }
        }

        public async Task<int?> GetVideoDurationAsync(string videoPath)
        {
            // Note: Để lấy thời lượng video chính xác, cần dùng thư viện như FFmpeg
            // Hoặc có thể lấy từ metadata khi upload
            // Tạm thời return null, có thể implement sau
            await Task.CompletedTask;
            return null;
        }
    }
}

