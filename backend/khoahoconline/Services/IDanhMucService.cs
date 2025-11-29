using khoahoconline.Dtos.DanhMuc;

namespace khoahoconline.Services
{
    public interface IDanhMucService
    {
        /// <summary>
        /// Lấy tất cả danh mục active (cho public)
        /// </summary>
        Task<List<DanhMucDetailDto>> GetAllActiveAsync();

        /// <summary>
        /// Lấy tất cả danh mục bao gồm cả inactive (cho admin)
        /// </summary>
        Task<List<DanhMucDetailDto>> GetAllAsync();

        /// <summary>
        /// Lấy chi tiết danh mục theo ID
        /// </summary>
        Task<DanhMucDetailDto> GetByIdAsync(int id);

        /// <summary>
        /// Tạo danh mục mới
        /// </summary>
        Task<DanhMucDto> CreateAsync(CreateDanhMucDto dto);

        /// <summary>
        /// Cập nhật danh mục
        /// </summary>
        Task UpdateAsync(int id, UpdateDanhMucDto dto);

        /// <summary>
        /// Xóa mềm danh mục (chỉ được xóa nếu không còn khóa học)
        /// </summary>
        Task SoftDeleteAsync(int id);

        /// <summary>
        /// Khôi phục danh mục đã xóa
        /// </summary>
        Task RestoreAsync(int id);

        /// <summary>
        /// Tìm kiếm danh mục
        /// </summary>
        Task<List<DanhMucDetailDto>> SearchAsync(string searchTerm);

        /// <summary>
        /// Đếm số khóa học trong danh mục
        /// </summary>
        Task<int> CountCoursesAsync(int danhMucId);
    }
}