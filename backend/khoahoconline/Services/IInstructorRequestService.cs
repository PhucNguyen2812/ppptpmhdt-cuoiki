using khoahoconline.Dtos;
using khoahoconline.Dtos.InstructorRequest;

namespace khoahoconline.Services;

public interface IInstructorRequestService
{
    /// <summary>
    /// Tạo yêu cầu đăng ký làm giảng viên
    /// </summary>
    Task<InstructorRequestDto> CreateRequestAsync(int userId, IFormFile chungChiFile, CreateInstructorRequestDto dto);

    /// <summary>
    /// Lấy danh sách yêu cầu (cho KIEMDUYETVIEN/ADMIN)
    /// </summary>
    Task<PagedResult<InstructorRequestDto>> GetRequestsAsync(string? trangThai = null, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Lấy chi tiết yêu cầu
    /// </summary>
    Task<InstructorRequestDto> GetRequestByIdAsync(int id);

    /// <summary>
    /// Lấy yêu cầu của học viên (nếu có)
    /// </summary>
    Task<InstructorRequestDto?> GetMyRequestAsync(int userId);

    /// <summary>
    /// Duyệt yêu cầu
    /// </summary>
    Task<bool> ApproveRequestAsync(int requestId, int approverId);

    /// <summary>
    /// Từ chối yêu cầu
    /// </summary>
    Task<bool> RejectRequestAsync(int requestId, int approverId, RejectInstructorRequestDto dto);
}








