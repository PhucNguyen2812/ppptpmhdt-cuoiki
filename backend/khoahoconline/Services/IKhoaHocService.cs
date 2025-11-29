using khoahoconline.Dtos;
using khoahoconline.Dtos.KhoaHoc;

namespace khoahoconline.Services
{
    public interface IKhoaHocService
    {
        /// <summary>
        /// Lấy danh sách khóa học với pagination, filter, search
        /// </summary>
        Task<PagedResult<KhoaHocDto>> GetPagedAsync(KhoaHocFilterDto filter);

        /// <summary>
        /// Lấy chi tiết khóa học
        /// </summary>
        Task<KhoaHocDetailDto> GetByIdAsync(int id);

        /// <summary>
        /// Lấy nội dung khóa học (curriculum)
        /// </summary>
        Task<CurriculumDto> GetCurriculumAsync(int id);

        /// <summary>
        /// Lấy khóa học nổi bật
        /// </summary>
        Task<List<KhoaHocDto>> GetFeaturedCoursesAsync(int take = 8);

        /// <summary>
        /// Lấy khóa học bán chạy nhất
        /// </summary>
        Task<List<KhoaHocDto>> GetBestSellingCoursesAsync(int take = 8);

        /// <summary>
        /// Lấy khóa học mới nhất
        /// </summary>
        Task<List<KhoaHocDto>> GetNewestCoursesAsync(int take = 8);

        /// <summary>
        /// Lấy khóa học của giảng viên
        /// </summary>
        Task<PagedResult<KhoaHocDto>> GetByInstructorAsync(int instructorId, int pageNumber = 1, int pageSize = 12);
    }
}