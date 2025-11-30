using Microsoft.AspNetCore.Http;

namespace khoahoconline.Services
{
    public interface IVideoUploadService
    {
        /// <summary>
        /// Upload video file và trả về URL
        /// </summary>
        Task<string> UploadVideoAsync(IFormFile file, string folder = "videos");

        /// <summary>
        /// Xóa video file
        /// </summary>
        Task<bool> DeleteVideoAsync(string videoUrl);

        /// <summary>
        /// Lấy thời lượng video (giây)
        /// </summary>
        Task<int?> GetVideoDurationAsync(string videoPath);
    }
}

