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
        /// Hiển thị lại khóa học (Instructor) - Cần duyệt lại
        /// </summary>
        Task<bool> UnhideCourseAsync(int courseId, int instructorId);

        /// <summary>
        /// Lấy danh sách khóa học chờ duyệt (Admin/Reviewer)
        /// </summary>
        Task<List<KiemDuyetKhoaHocDto>> GetPendingCoursesAsync();

        /// <summary>
        /// Lấy tất cả khóa học với trạng thái kiểm duyệt (Admin/Reviewer)
        /// </summary>
        Task<List<KiemDuyetKhoaHocDto>> GetAllCourseApprovalsAsync(string? status = null);

        /// <summary>
        /// Duyệt khóa học (Admin/Reviewer)
        /// </summary>
        Task<bool> ApproveCourseAsync(int courseId, ApproveCourseDto dto, int reviewerId);

        /// <summary>
        /// Từ chối khóa học (Admin/Reviewer)
        /// </summary>
        Task<bool> RejectCourseAsync(int courseId, RejectCourseDto dto, int reviewerId);

        /// <summary>
        /// Ẩn khóa học (Admin/Reviewer) - Tạo approval record với status BiAn
        /// </summary>
        Task<bool> HideCourseByAdminAsync(int courseId, ApproveCourseDto dto, int reviewerId);

        /// <summary>
        /// Bỏ ẩn khóa học (Admin/Reviewer) - Tạo approval record với status DaDuyet và hiển thị lại
        /// </summary>
        Task<bool> UnhideCourseByAdminAsync(int courseId, ApproveCourseDto dto, int reviewerId);

        /// <summary>
        /// Lấy khóa học với curriculum cho instructor để chỉnh sửa
        /// </summary>
        Task<CreateCourseWithCurriculumDto> GetCourseForEditAsync(int courseId, int instructorId);

        /// <summary>
        /// Cập nhật khóa học kèm chương và bài giảng (Instructor)
        /// </summary>
        Task<KhoaHocDto> UpdateCourseWithCurriculumAsync(int courseId, UpdateCourseWithCurriculumDto dto, int instructorId);

        /// <summary>
        /// Lấy khóa học với curriculum cho admin/reviewer để xem (không cần ownership)
        /// </summary>
        Task<CreateCourseWithCurriculumDto> GetCourseForReviewAsync(int courseId);
    }
}