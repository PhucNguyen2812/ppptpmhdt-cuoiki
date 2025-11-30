using khoahoconline.Data.Entities;

namespace khoahoconline.Data.Repositories
{
    public interface IKiemDuyetKhoaHocRepository : IBaseRepository<KiemDuyetKhoaHoc>
    {
        /// <summary>
        /// Lấy bản kiểm duyệt mới nhất của khóa học
        /// </summary>
        Task<KiemDuyetKhoaHoc?> GetLatestByCourseIdAsync(int courseId);

        /// <summary>
        /// Lấy bản kiểm duyệt theo khóa học và phiên bản
        /// </summary>
        Task<KiemDuyetKhoaHoc?> GetByCourseIdAndVersionAsync(int courseId, int version);

        /// <summary>
        /// Lấy danh sách khóa học chờ kiểm duyệt
        /// </summary>
        Task<List<KiemDuyetKhoaHoc>> GetPendingApprovalsAsync();

        /// <summary>
        /// Kiểm tra xem khóa học có đang chờ kiểm duyệt không
        /// </summary>
        Task<bool> IsCoursePendingAsync(int courseId);

        /// <summary>
        /// Lấy tất cả các bản kiểm duyệt với filter theo trạng thái (lấy bản mới nhất của mỗi khóa học)
        /// </summary>
        Task<List<KiemDuyetKhoaHoc>> GetAllApprovalsAsync(string? status = null);
    }
}


