using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using khoahoconline.Dtos;
using khoahoconline.Dtos.NguoiDung;
using khoahoconline.Services;
using System.Security.Claims;

namespace khoahoconline.Controllers
{
    [Route("api/v1/nguoidungs")]
    [ApiController]
    public class NguoiDungsController : ControllerBase
    {
        private readonly ILogger<NguoiDungsController> _logger;
        private readonly INguoiDungService _nguoiDungService;
        
        public NguoiDungsController(ILogger<NguoiDungsController> logger, INguoiDungService nguoiDungService)
        {
            _logger = logger;
            _nguoiDungService = nguoiDungService;
        }

        /// <summary>
        /// [ADMIN] Tạo người dùng mới
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateNguoiDung([FromBody] CreateNguoiDungDto dto)
        {
            _logger.LogInformation("Creating new NguoiDung");
            var result = ApiResponse<NguoiDungDto>.SuccessResponse(await _nguoiDungService.createAsync(dto));
            return CreatedAtAction(nameof(GetNguoiDungById), new { id = result.Data?.Id }, result);
        }

        /// <summary>
        /// [ADMIN] Lấy thông tin người dùng theo ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetNguoiDungById(int id)
        {
            _logger.LogInformation($"Getting NguoiDung by id: {id}");
            var result = ApiResponse<NguoiDungDto>.SuccessResponse(await _nguoiDungService.getByIdAsync(id));
            return Ok(result);
        }

        /// <summary>
        /// [ADMIN] Lấy thông tin chi tiết người dùng (kèm vai trò)
        /// </summary>
        [HttpGet("{id}/detail")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetNguoiDungDetail(int id)
        {
            _logger.LogInformation($"Getting NguoiDung detail by id: {id}");
            var result = ApiResponse<NguoiDungDetailDto>.SuccessResponse(await _nguoiDungService.GetDetailByIdAsync(id));
            return Ok(result);
        }

        /// <summary>
        /// [ADMIN] Lấy danh sách người dùng (có phân trang và tìm kiếm)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllNguoiDungs(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] bool active = true,
            [FromQuery] string? searchTerm = null)
        {
            _logger.LogInformation("Getting all NguoiDungs");
            var result = ApiResponse<PagedResult<NguoiDungDto>>.SuccessResponse(
                await _nguoiDungService.GetAllAsync(pageNumber, pageSize, active, searchTerm));
            return Ok(result);
        }

        /// <summary>
        /// [ADMIN] Vô hiệu hóa người dùng (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteNguoiDung(int id)
        {
            _logger.LogInformation($"Soft deleting NguoiDung with id: {id}");
            await _nguoiDungService.SoftDeleteAsync(id);
            var result = ApiResponse<string>.SuccessResponse("Vô hiệu hóa người dùng thành công");
            return Ok(result);
        }

        /// <summary>
        /// [ADMIN] Khôi phục người dùng đã vô hiệu hóa
        /// </summary>
        [HttpPut("{id}/restore")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> RestoreNguoiDung(int id)
        {
            _logger.LogInformation($"Restoring NguoiDung with id: {id}");
            await _nguoiDungService.RestoreAsync(id);
            var result = ApiResponse<string>.SuccessResponse("Khôi phục người dùng thành công");
            return Ok(result);
        }

        /// <summary>
        /// [ADMIN] Cập nhật thông tin người dùng
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateNguoiDung(int id, [FromBody] UpdateNguoiDungDto dto)
        {
            _logger.LogInformation($"Updating NguoiDung with id: {id}");
            if (dto == null) 
            { 
                throw new ArgumentNullException(nameof(dto));
            }

            await _nguoiDungService.updateAsync(id, dto);
            return NoContent();
        }

        /// <summary>
        /// [USER] Lấy thông tin profile của chính mình
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            _logger.LogInformation($"User {userId} getting their profile");
            
            var result = ApiResponse<NguoiDungDetailDto>.SuccessResponse(
                await _nguoiDungService.GetDetailByIdAsync(userId));
            return Ok(result);
        }

        /// <summary>
        /// [USER] Cập nhật profile của chính mình
        /// </summary>
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            _logger.LogInformation($"User {userId} updating their profile");
            
            await _nguoiDungService.UpdateProfileAsync(userId, dto);
            var result = ApiResponse<string>.SuccessResponse("Cập nhật profile thành công");
            return Ok(result);
        }

        /// <summary>
        /// [USER] Đổi mật khẩu
        /// </summary>
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            _logger.LogInformation($"User {userId} changing password");
            
            await _nguoiDungService.ChangePasswordAsync(userId, dto);
            var result = ApiResponse<string>.SuccessResponse("Đổi mật khẩu thành công");
            return Ok(result);
        }

        /// <summary>
        /// [USER] Upload avatar
        /// </summary>
        [HttpPost("avatar")]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            _logger.LogInformation($"User {userId} uploading avatar");
            
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("File không hợp lệ"));
            }

            var avatarUrl = await _nguoiDungService.UploadAvatarAsync(userId, file);
            var result = ApiResponse<string>.SuccessResponse(avatarUrl, "Upload avatar thành công");
            return Ok(result);
        }

        /// <summary>
        /// [USER] Đăng ký làm giảng viên
        /// </summary>
        [HttpPost("register-instructor")]
        [Authorize(Roles = "HOCVIEN")]
        public async Task<IActionResult> RegisterAsInstructor()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            _logger.LogInformation($"User {userId} registering as instructor");
            
            var success = await _nguoiDungService.RegisterAsInstructorAsync(userId);
            
            if (success)
            {
                var result = ApiResponse<string>.SuccessResponse(
                    "Đăng ký làm giảng viên thành công! Bạn có thể bắt đầu tạo khóa học.");
                return Ok(result);
            }
            
            return BadRequest(ApiResponse<string>.ErrorResponse("Đăng ký làm giảng viên thất bại"));
        }
    }
}