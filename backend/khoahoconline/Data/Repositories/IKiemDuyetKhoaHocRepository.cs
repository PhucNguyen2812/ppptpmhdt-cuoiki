using khoahoconline.Data.Entities;
using khoahoconline.Dtos;

namespace khoahoconline.Data.Repositories
{
    public interface IKiemDuyetKhoaHocRepository : IBaseRepository<KiemDuyetKhoaHoc>
    {
        /// <summary>
        /// Lấy danh sách yêu cầu kiểm duyệt với pagination và filter
        /// </summary>
        Task<PagedResult<KiemDuyetKhoaHoc>> GetPagedAsync(string? trangThai = null, int pageNumber = 1, int pageSize = 10);

        /// <summary>
        /// Lấy yêu cầu kiểm duyệt theo ID kèm thông tin khóa học, giảng viên và người duyệt
        /// </summary>
        Task<KiemDuyetKhoaHoc?> GetByIdWithDetailsAsync(int id);

        /// <summary>
        /// Lấy yêu cầu kiểm duyệt của khóa học (nếu có)
        /// </summary>
        Task<KiemDuyetKhoaHoc?> GetByKhoaHocIdAsync(int khoaHocId);

        /// <summary>
        /// Kiểm tra khóa học đã có yêu cầu chờ duyệt chưa
        /// </summary>
        Task<bool> HasPendingRequestAsync(int khoaHocId);
    }
}








