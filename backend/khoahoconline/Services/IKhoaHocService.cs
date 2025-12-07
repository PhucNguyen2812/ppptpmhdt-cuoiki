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
        /// <param name="id">ID khóa học</param>
        /// <param name="userId">ID người dùng (null nếu chưa đăng nhập)</param>
        Task<CurriculumDto> GetCurriculumAsync(int id, int? userId = null);

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

        /// <summary>
        /// Tạo khóa học mới (Instructor)
        /// </summary>
        Task<KhoaHocDto> CreateCourseAsync(CreateCourseDto dto, int instructorId);

        /// <summary>
        /// Tạo khóa học mới kèm chương và bài giảng (Instructor)
        /// </summary>
        Task<KhoaHocDto> CreateCourseWithCurriculumAsync(CreateCourseWithCurriculumDto dto, int instructorId);

        /// <summary>
        /// Cập nhật khóa học (Instructor)
        /// </summary>
        Task<KhoaHocDto> UpdateCourseAsync(int courseId, UpdateCourseDto dto, int instructorId);

        /// <summary>
        /// Xóa khóa học (Instructor) - Soft delete
        /// </summary>
        Task<bool> DeleteCourseAsync(int courseId, int instructorId);

        /// <summary>
        /// Ẩn khóa học (Instructor)
        /// </summary>
        Task<bool> HideCourseAsync(int courseId, int instructorId);

        /// <summary>
        /// Hiển thị lại khóa học (Instructor)
        /// </summary>
        Task<bool> UnhideCourseAsync(int courseId, int instructorId);


        /// <summary>
        /// Lấy khóa học với curriculum cho instructor để chỉnh sửa
        /// </summary>
        Task<CreateCourseWithCurriculumDto> GetCourseForEditAsync(int courseId, int instructorId);

        /// <summary>
        /// Cập nhật khóa học kèm chương và bài giảng (Instructor)
        /// </summary>
        Task<KhoaHocDto> UpdateCourseWithCurriculumAsync(int courseId, UpdateCourseWithCurriculumDto dto, int instructorId);

    }
}