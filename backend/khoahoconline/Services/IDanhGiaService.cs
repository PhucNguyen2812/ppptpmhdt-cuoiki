using khoahoconline.Dtos;
using khoahoconline.Dtos.Review;

namespace khoahoconline.Services
{
    public interface IDanhGiaService
    {
        Task<ReviewDto> CreateReviewAsync(int userId, CreateReviewDto dto);
        Task<ReviewDto> UpdateReviewAsync(int reviewId, int userId, UpdateReviewDto dto);
        Task<bool> DeleteReviewAsync(int reviewId, int userId);
        Task<ReviewDto?> GetMyReviewAsync(int courseId, int userId);
        Task<PagedResult<ReviewDto>> GetReviewsByCourseIdAsync(int courseId, ReviewFilterDto filter);
        Task<CourseReviewSummaryDto> GetCourseReviewSummaryAsync(int courseId);
    }
}




