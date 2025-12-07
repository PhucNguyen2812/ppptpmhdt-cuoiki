using khoahoconline.Data.Entities;

namespace khoahoconline.Data.Repositories
{
    public interface IDanhMucRepository : IBaseRepository<DanhMucKhoaHoc>
    {
        /// <summary>
        /// Lấy tất cả danh mục active, sắp xếp theo tên A-Z
        /// </summary>
        Task<List<DanhMucKhoaHoc>> GetAllActiveAsync();

        /// <summary>
        /// Lấy tất cả danh mục (bao gồm cả inactive), sắp xếp theo tên A-Z
        /// </summary>
        Task<List<DanhMucKhoaHoc>> GetAllAsync();

        /// <summary>
        /// Lấy danh mục theo ID
        /// </summary>
        new Task<DanhMucKhoaHoc?> GetByIdAsync(int id);

        /// <summary>
        /// Lấy danh mục theo ID kèm danh sách khóa học
        /// </summary>
        Task<DanhMucKhoaHoc?> GetByIdWithCoursesAsync(int id);

        /// <summary>
        /// Kiểm tra tên danh mục đã tồn tại chưa
        /// </summary>
        Task<bool> IsTenDanhMucExistsAsync(string tenDanhMuc, int? excludeId = null);

        /// <summary>
        /// Đếm số lượng khóa học trong danh mục
        /// </summary>
        Task<int> CountCoursesAsync(int danhMucId);

        /// <summary>
        /// Tìm kiếm danh mục theo tên
        /// </summary>
        Task<List<DanhMucKhoaHoc>> SearchAsync(string searchTerm);

        /// <summary>
        /// Soft delete danh mục
        /// </summary>
        Task SoftDeleteAsync(DanhMucKhoaHoc entity);
    }
}