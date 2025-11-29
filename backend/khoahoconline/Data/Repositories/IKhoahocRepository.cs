using khoahoconline.Data.Entities;
using khoahoconline.Dtos.KhoaHoc;
using khoahoconline.Dtos;

namespace khoahoconline.Data.Repositories
{
    public interface IKhoaHocRepository : IBaseRepository<KhoaHoc>
    {
        /// <summary>
        /// Lấy danh sách khóa học có pagination, filter, search, sort
        /// </summary>
        Task<PagedResult<KhoaHoc>> GetPagedAsync(KhoaHocFilterDto filter);

        /// <summary>
        /// Lấy khóa học theo ID kèm navigation properties
        /// </summary>
        Task<KhoaHoc?> GetByIdWithDetailsAsync(int id);

        /// <summary>
        /// Lấy khóa học theo ID kèm curriculum (chapters + lectures)
        /// </summary>
        Task<KhoaHoc?> GetByIdWithCurriculumAsync(int id);

        /// <summary>
        /// Lấy khóa học nổi bật (rating >= 4.5, có học viên)
        /// </summary>
        Task<List<KhoaHoc>> GetFeaturedCoursesAsync(int take = 8);

        /// <summary>
        /// Lấy khóa học bán chạy nhất
        /// </summary>
        Task<List<KhoaHoc>> GetBestSellingCoursesAsync(int take = 8);

        /// <summary>
        /// Lấy khóa học mới nhất
        /// </summary>
        Task<List<KhoaHoc>> GetNewestCoursesAsync(int take = 8);

        /// <summary>
        /// Lấy khóa học của giảng viên
        /// </summary>
        Task<PagedResult<KhoaHoc>> GetByInstructorAsync(int instructorId, int pageNumber = 1, int pageSize = 12);

        /// <summary>
        /// Đếm số khóa học của giảng viên
        /// </summary>
        Task<int> CountByInstructorAsync(int instructorId);

        /// <summary>
        /// Tính tổng thời lượng khóa học (phút)
        /// </summary>
        Task<int> GetTotalDurationAsync(int courseId);

        /// <summary>
        /// Đếm tổng số chương
        /// </summary>
        Task<int> CountChaptersAsync(int courseId);

        /// <summary>
        /// Đếm tổng số bài giảng
        /// </summary>
        Task<int> CountLecturesAsync(int courseId);
    }
}