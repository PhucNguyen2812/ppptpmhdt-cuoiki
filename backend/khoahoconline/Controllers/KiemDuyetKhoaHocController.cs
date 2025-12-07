using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using khoahoconline.Dtos;
using khoahoconline.Dtos.KiemDuyetKhoaHoc;
using khoahoconline.Services;

namespace khoahoconline.Controllers;

[ApiController]
[Route("api/v1/course-approvals")]
public class KiemDuyetKhoaHocController : ControllerBase
{
    private readonly IKiemDuyetKhoaHocService _kiemDuyetKhoaHocService;
    private readonly ILogger<KiemDuyetKhoaHocController> _logger;

    public KiemDuyetKhoaHocController(
        IKiemDuyetKhoaHocService kiemDuyetKhoaHocService,
        ILogger<KiemDuyetKhoaHocController> logger)
    {
        _kiemDuyetKhoaHocService = kiemDuyetKhoaHocService;
        _logger = logger;
    }

    /// <summary>
    /// [GIANGVIEN] Gửi yêu cầu kiểm duyệt khóa học
    /// </summary>
    [HttpPost("request/{khoaHocId}")]
    [Authorize(Roles = "GIANGVIEN")]
    public async Task<IActionResult> CreateApprovalRequest(int khoaHocId)
    {
        var instructorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        if (instructorId == 0)
        {
            return Unauthorized(ApiResponse<string>.ErrorResponse("Không tìm thấy thông tin người dùng"));
        }

        try
        {
            var result = await _kiemDuyetKhoaHocService.CreateApprovalRequestAsync(khoaHocId, instructorId);
            return Ok(ApiResponse<KiemDuyetKhoaHocDto>.SuccessResponse(result, "Yêu cầu kiểm duyệt đã được gửi"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating course approval request for course {khoaHocId}");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// [ADMIN] Lấy danh sách yêu cầu kiểm duyệt
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetApprovalRequests([FromQuery] string? trangThai = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _kiemDuyetKhoaHocService.GetApprovalRequestsAsync(trangThai, pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<KiemDuyetKhoaHocDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course approval requests");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// [ADMIN] Lấy chi tiết yêu cầu kiểm duyệt
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetApprovalRequestById(int id)
    {
        try
        {
            var result = await _kiemDuyetKhoaHocService.GetApprovalRequestByIdAsync(id);
            return Ok(ApiResponse<KiemDuyetKhoaHocDetailDto>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting course approval request {id}");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// [ADMIN] Duyệt khóa học
    /// </summary>
    [HttpPost("{id}/approve")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> ApproveCourse(int id, [FromBody] ApproveCourseApprovalDto? dto = null)
    {
        var approverId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        if (approverId == 0)
        {
            return Unauthorized(ApiResponse<string>.ErrorResponse("Không tìm thấy thông tin người dùng"));
        }

        try
        {
            var result = await _kiemDuyetKhoaHocService.ApproveCourseAsync(id, approverId, dto);
            return Ok(ApiResponse<string>.SuccessResponse("Đã duyệt khóa học thành công"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error approving course {id}");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// [ADMIN] Từ chối khóa học
    /// </summary>
    [HttpPost("{id}/reject")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> RejectCourse(int id, [FromBody] RejectCourseApprovalDto dto)
    {
        var approverId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        if (approverId == 0)
        {
            return Unauthorized(ApiResponse<string>.ErrorResponse("Không tìm thấy thông tin người dùng"));
        }

        try
        {
            var result = await _kiemDuyetKhoaHocService.RejectCourseAsync(id, approverId, dto);
            return Ok(ApiResponse<string>.SuccessResponse("Đã từ chối khóa học"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error rejecting course {id}");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }
}








