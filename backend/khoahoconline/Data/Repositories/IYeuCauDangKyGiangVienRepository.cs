using khoahoconline.Data.Entities;
using khoahoconline.Dtos;

namespace khoahoconline.Data.Repositories
{
    public interface IYeuCauDangKyGiangVienRepository : IBaseRepository<YeuCauDangKyGiangVien>
    {
        /// <summary>
        /// Lấy danh sách yêu cầu với pagination và filter
        /// </summary>
        Task<PagedResult<YeuCauDangKyGiangVien>> GetPagedAsync(string? trangThai = null, int pageNumber = 1, int pageSize = 10);

        /// <summary>
        /// Lấy yêu cầu theo ID kèm thông tin học viên và người duyệt
        /// </summary>
        Task<YeuCauDangKyGiangVien?> GetByIdWithDetailsAsync(int id);

        /// <summary>
        /// Lấy yêu cầu của học viên (nếu có)
        /// </summary>
        Task<YeuCauDangKyGiangVien?> GetByHocVienIdAsync(int hocVienId);

        /// <summary>
        /// Kiểm tra học viên đã có yêu cầu chờ duyệt chưa
        /// </summary>
        Task<bool> HasPendingRequestAsync(int hocVienId);
    }
}








