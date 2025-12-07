using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using khoahoconline.Dtos;
using khoahoconline.Dtos.InstructorRequest;
using khoahoconline.Services;

namespace khoahoconline.Controllers;

[ApiController]
[Route("api/v1/instructor-requests")]
public class InstructorRequestController : ControllerBase
{
    private readonly IInstructorRequestService _instructorRequestService;
    private readonly ILogger<InstructorRequestController> _logger;

    public InstructorRequestController(
        IInstructorRequestService instructorRequestService,
        ILogger<InstructorRequestController> logger)
    {
        _instructorRequestService = instructorRequestService;
        _logger = logger;
    }

    /// <summary>
    /// [AUTH] Gửi yêu cầu đăng ký làm giảng viên
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "HOCVIEN")]
    public async Task<IActionResult> CreateRequest([FromForm] CreateInstructorRequestDto dto, [FromForm] IFormFile chungChi)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        if (userId == 0)
        {
            return Unauthorized(ApiResponse<string>.ErrorResponse("Không tìm thấy thông tin người dùng"));
        }

        try
        {
            var result = await _instructorRequestService.CreateRequestAsync(userId, chungChi, dto);
            return Ok(ApiResponse<InstructorRequestDto>.SuccessResponse(result, "Yêu cầu đã được gửi"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating instructor request");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// [AUTH] Lấy yêu cầu của mình
    /// </summary>
    [HttpGet("my-request")]
    [Authorize(Roles = "HOCVIEN")]
    public async Task<IActionResult> GetMyRequest()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        if (userId == 0)
        {
            return Unauthorized(ApiResponse<string>.ErrorResponse("Không tìm thấy thông tin người dùng"));
        }

        try
        {
            var result = await _instructorRequestService.GetMyRequestAsync(userId);
            if (result == null)
            {
                return Ok(ApiResponse<InstructorRequestDto?>.SuccessResponse(null, "Bạn chưa có yêu cầu nào"));
            }
            return Ok(ApiResponse<InstructorRequestDto>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting my instructor request");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// [AUTH] Lấy danh sách yêu cầu (cho KIEMDUYETVIEN/ADMIN)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "KIEMDUYETVIEN,ADMIN")]
    public async Task<IActionResult> GetRequests([FromQuery] string? trangThai = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _instructorRequestService.GetRequestsAsync(trangThai, pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<InstructorRequestDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting instructor requests");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// [AUTH] Lấy chi tiết yêu cầu
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "KIEMDUYETVIEN,ADMIN")]
    public async Task<IActionResult> GetRequestById(int id)
    {
        try
        {
            var result = await _instructorRequestService.GetRequestByIdAsync(id);
            return Ok(ApiResponse<InstructorRequestDto>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting instructor request {id}");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// [AUTH] Duyệt yêu cầu
    /// </summary>
    [HttpPost("{id}/approve")]
    [Authorize(Roles = "KIEMDUYETVIEN,ADMIN")]
    public async Task<IActionResult> ApproveRequest(int id)
    {
        var approverId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        if (approverId == 0)
        {
            return Unauthorized(ApiResponse<string>.ErrorResponse("Không tìm thấy thông tin người dùng"));
        }

        try
        {
            var result = await _instructorRequestService.ApproveRequestAsync(id, approverId);
            return Ok(ApiResponse<string>.SuccessResponse("Đã duyệt yêu cầu thành công"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error approving instructor request {id}");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// [AUTH] Từ chối yêu cầu
    /// </summary>
    [HttpPost("{id}/reject")]
    [Authorize(Roles = "KIEMDUYETVIEN,ADMIN")]
    public async Task<IActionResult> RejectRequest(int id, [FromBody] RejectInstructorRequestDto dto)
    {
        var approverId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        if (approverId == 0)
        {
            return Unauthorized(ApiResponse<string>.ErrorResponse("Không tìm thấy thông tin người dùng"));
        }

        try
        {
            var result = await _instructorRequestService.RejectRequestAsync(id, approverId, dto);
            return Ok(ApiResponse<string>.SuccessResponse("Đã từ chối yêu cầu"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error rejecting instructor request {id}");
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
    }
}








