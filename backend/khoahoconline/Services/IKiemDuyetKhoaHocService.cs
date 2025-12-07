using khoahoconline.Dtos;
using khoahoconline.Dtos.KiemDuyetKhoaHoc;

namespace khoahoconline.Services;

public interface IKiemDuyetKhoaHocService
{
    /// <summary>
    /// Tạo yêu cầu kiểm duyệt khóa học (khi giảng viên muốn publish)
    /// </summary>
    Task<KiemDuyetKhoaHocDto> CreateApprovalRequestAsync(int khoaHocId, int instructorId);

    /// <summary>
    /// Lấy danh sách yêu cầu kiểm duyệt (cho ADMIN)
    /// </summary>
    Task<PagedResult<KiemDuyetKhoaHocDto>> GetApprovalRequestsAsync(string? trangThai = null, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Lấy chi tiết yêu cầu kiểm duyệt
    /// </summary>
    Task<KiemDuyetKhoaHocDetailDto> GetApprovalRequestByIdAsync(int id);

    /// <summary>
    /// Duyệt khóa học
    /// </summary>
    Task<bool> ApproveCourseAsync(int requestId, int approverId, ApproveCourseApprovalDto? dto = null);

    /// <summary>
    /// Từ chối khóa học
    /// </summary>
    Task<bool> RejectCourseAsync(int requestId, int approverId, RejectCourseApprovalDto dto);
}








