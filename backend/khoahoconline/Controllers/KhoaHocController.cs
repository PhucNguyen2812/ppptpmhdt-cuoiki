using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;
using khoahoconline.Data.Repositories;
using khoahoconline.Dtos;
using khoahoconline.Dtos.KhoaHoc;
using khoahoconline.Services;

namespace khoahoconline.Controllers
{
    [Route("api/v1/courses")]
    [ApiController]
    public class KhoaHocController : ControllerBase
    {
        private readonly ILogger<KhoaHocController> _logger;
        private readonly IKhoaHocService _khoaHocService;
        private readonly IVideoUploadService _videoUploadService;
        private readonly IUnitOfWork _unitOfWork;

        public KhoaHocController(
            ILogger<KhoaHocController> logger,
            IKhoaHocService khoaHocService,
            IVideoUploadService videoUploadService,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _khoaHocService = khoaHocService;
            _videoUploadService = videoUploadService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// [PUBLIC] Lấy danh sách khóa học với filter, search, sort, pagination
        /// </summary>
        /// <param name="filter">Filter parameters</param>
        /// <returns>Danh sách khóa học</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] KhoaHocFilterDto filter)
        {
            _logger.LogInformation("Getting all courses with filters");
            var result = await _khoaHocService.GetPagedAsync(filter);
            return Ok(ApiResponse<PagedResult<KhoaHocDto>>.SuccessResponse(result));
        }

        /// <summary>
        /// [PUBLIC] Lấy khóa học nổi bật
        /// </summary>
        /// <param name="take">Số lượng khóa học cần lấy (default: 8)</param>
        /// <returns>Danh sách khóa học nổi bật</returns>
        [HttpGet("featured")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeatured([FromQuery] int take = 8)
        {
            _logger.LogInformation($"Getting featured courses, take: {take}");
            var result = await _khoaHocService.GetFeaturedCoursesAsync(take);
            return Ok(ApiResponse<List<KhoaHocDto>>.SuccessResponse(
                result, 
                $"Lấy {result.Count} khóa học nổi bật thành công"));
        }

        /// <summary>
        /// [PUBLIC] Lấy khóa học bán chạy nhất
        /// </summary>
        /// <param name="take">Số lượng khóa học cần lấy (default: 8)</param>
        /// <returns>Danh sách khóa học bán chạy</returns>
        [HttpGet("best-selling")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBestSelling([FromQuery] int take = 8)
        {
            _logger.LogInformation($"Getting best-selling courses, take: {take}");
            var result = await _khoaHocService.GetBestSellingCoursesAsync(take);
            return Ok(ApiResponse<List<KhoaHocDto>>.SuccessResponse(
                result,
                $"Lấy {result.Count} khóa học bán chạy thành công"));
        }

        /// <summary>
        /// [PUBLIC] Lấy khóa học mới nhất
        /// </summary>
        /// <param name="take">Số lượng khóa học cần lấy (default: 8)</param>
        /// <returns>Danh sách khóa học mới nhất</returns>
        [HttpGet("newest")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewest([FromQuery] int take = 8)
        {
            _logger.LogInformation($"Getting newest courses, take: {take}");
            var result = await _khoaHocService.GetNewestCoursesAsync(take);
            return Ok(ApiResponse<List<KhoaHocDto>>.SuccessResponse(
                result,
                $"Lấy {result.Count} khóa học mới nhất thành công"));
        }

        /// <summary>
        /// [PUBLIC] Lấy khóa học của giảng viên
        /// </summary>
        /// <param name="instructorId">ID giảng viên</param>
        /// <param name="pageNumber">Trang số (default: 1)</param>
        /// <param name="pageSize">Số items mỗi trang (default: 12)</param>
        /// <returns>Danh sách khóa học của giảng viên</returns>
        [HttpGet("instructor/{instructorId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByInstructor(
            int instructorId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 12)
        {
            _logger.LogInformation($"Getting courses by instructor: {instructorId}, pageNumber: {pageNumber}, pageSize: {pageSize}");
            
            try
            {
                // Validate input
                if (instructorId <= 0)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse("ID giảng viên không hợp lệ"));
                }

                if (pageNumber < 1)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse("Số trang phải lớn hơn 0"));
                }

                if (pageSize < 1 || pageSize > 100)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse("Số items mỗi trang phải từ 1 đến 100"));
                }

                var result = await _khoaHocService.GetByInstructorAsync(instructorId, pageNumber, pageSize);
                return Ok(ApiResponse<PagedResult<KhoaHocDto>>.SuccessResponse(result));
            }
            catch (Middleware.Exceptions.NotFoundException ex)
            {
                _logger.LogWarning(ex, $"Instructor not found: {instructorId}");
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting courses by instructor: {instructorId}");
                return StatusCode(500, ApiResponse<string>.ErrorResponse($"Lỗi khi lấy danh sách khóa học: {ex.Message}"));
            }
        }


        /// <summary>
        /// [PUBLIC] Lấy chi tiết khóa học
        /// </summary>
        /// <param name="id">ID khóa học</param>
        /// <returns>Chi tiết khóa học</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation($"Getting course detail: {id}");
            var result = await _khoaHocService.GetByIdAsync(id);
            return Ok(ApiResponse<KhoaHocDetailDto>.SuccessResponse(result));
        }

        /// <summary>
        /// [PUBLIC] Lấy nội dung khóa học (curriculum)
        /// </summary>
        /// <param name="id">ID khóa học</param>
        /// <returns>Danh sách chương và bài giảng</returns>
        [HttpGet("{id}/curriculum")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCurriculum(int id)
        {
            _logger.LogInformation($"Getting curriculum for course: {id}");
            
            // Lấy userId nếu user đã đăng nhập (để kiểm tra quyền truy cập)
            int? userId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                try
                {
                    userId = GetUserId();
                }
                catch
                {
                    // Nếu không lấy được userId, để null (user chưa đăng nhập hoặc token không hợp lệ)
                    userId = null;
                }
            }
            
            var result = await _khoaHocService.GetCurriculumAsync(id, userId);
            return Ok(ApiResponse<CurriculumDto>.SuccessResponse(result));
        }

        /// <summary>
        /// [GIANGVIEN] Upload video (generic endpoint)
        /// <summary>
        /// Upload video - Chỉ cần authenticated, không cần role cụ thể
        /// </summary>
        /// <param name="video">File video</param>
        /// <param name="folder">Thư mục lưu trữ (default: videos)</param>
        /// <returns>URL của video đã upload</returns>
        [HttpPost("upload-video")]
        [Authorize] // Chỉ cần đăng nhập, không cần role cụ thể
        [RequestFormLimits(MultipartBodyLengthLimit = 524288000)] // 500MB
        [RequestSizeLimit(524288000)] // 500MB
        public async Task<IActionResult> UploadVideo(IFormFile video, [FromQuery] string folder = "videos")
        {
            // Kiểm tra file
            if (video == null || video.Length == 0)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("File video không hợp lệ"));
            }

            try
            {
                var videoUrl = await _videoUploadService.UploadVideoAsync(video, folder);
                _logger.LogInformation($"Video uploaded successfully: {videoUrl}");
                return Ok(ApiResponse<string>.SuccessResponse(videoUrl, "Upload video thành công"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading video");
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// [GIANGVIEN] Tạo khóa học mới
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "GIANGVIEN")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
        {
            _logger.LogInformation("Creating new course");
            try
            {
                var instructorId = GetUserId();
                var result = await _khoaHocService.CreateCourseAsync(dto, instructorId);
                return Ok(ApiResponse<KhoaHocDto>.SuccessResponse(result, "Tạo khóa học thành công."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// [GIANGVIEN] Tạo khóa học mới kèm chương và bài giảng
        /// </summary>
        [HttpPost("with-curriculum")]
        [Authorize(Roles = "GIANGVIEN")]
        public async Task<IActionResult> CreateCourseWithCurriculum([FromBody] CreateCourseWithCurriculumDto dto)
        {
            _logger.LogInformation("Creating new course with curriculum");
            try
            {
                var instructorId = GetUserId();
                var result = await _khoaHocService.CreateCourseWithCurriculumAsync(dto, instructorId);
                return Ok(ApiResponse<KhoaHocDto>.SuccessResponse(result, "Tạo khóa học thành công."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course with curriculum");
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// [GIANGVIEN] Cập nhật khóa học
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "GIANGVIEN")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseDto dto)
        {
            _logger.LogInformation($"Updating course: {id}");
            try
            {
                var instructorId = GetUserId();
                var result = await _khoaHocService.UpdateCourseAsync(id, dto, instructorId);
                return Ok(ApiResponse<KhoaHocDto>.SuccessResponse(result, "Cập nhật khóa học thành công."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating course {id}");
                if (ex is Middleware.Exceptions.NotFoundException || ex is Middleware.Exceptions.UnauthorizedException)
                {
                    return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
                }
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// [GIANGVIEN] Lấy khóa học với curriculum để chỉnh sửa
        /// </summary>
        [HttpGet("{id}/for-edit")]
        [Authorize(Roles = "GIANGVIEN")]
        public async Task<IActionResult> GetCourseForEdit(int id)
        {
            _logger.LogInformation($"Getting course for edit: {id}");
            try
            {
                var instructorId = GetUserId();
                var result = await _khoaHocService.GetCourseForEditAsync(id, instructorId);
                return Ok(ApiResponse<CreateCourseWithCurriculumDto>.SuccessResponse(result, "Lấy thông tin khóa học thành công"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting course for edit {id}");
                if (ex is Middleware.Exceptions.NotFoundException || ex is Middleware.Exceptions.UnauthorizedException)
                {
                    return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
                }
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }


        /// <summary>
        /// [GIANGVIEN] Cập nhật khóa học kèm chương và bài giảng
        /// </summary>
        [HttpPut("{id}/with-curriculum")]
        [Authorize(Roles = "GIANGVIEN")]
        public async Task<IActionResult> UpdateCourseWithCurriculum(int id, [FromBody] UpdateCourseWithCurriculumDto dto)
        {
            _logger.LogInformation($"Updating course with curriculum: {id}");
            try
            {
                var instructorId = GetUserId();
                var result = await _khoaHocService.UpdateCourseWithCurriculumAsync(id, dto, instructorId);
                return Ok(ApiResponse<KhoaHocDto>.SuccessResponse(result, "Cập nhật khóa học thành công."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating course with curriculum {id}");
                if (ex is Middleware.Exceptions.NotFoundException || ex is Middleware.Exceptions.UnauthorizedException)
                {
                    return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
                }
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// [GIANGVIEN] Xóa khóa học
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "GIANGVIEN")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            _logger.LogInformation($"Deleting course: {id}");
            try
            {
                var instructorId = GetUserId();
                var result = await _khoaHocService.DeleteCourseAsync(id, instructorId);
                return Ok(ApiResponse<bool>.SuccessResponse(result, "Xóa khóa học thành công"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting course {id}");
                if (ex is Middleware.Exceptions.NotFoundException || ex is Middleware.Exceptions.UnauthorizedException)
                {
                    return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
                }
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// [GIANGVIEN] Ẩn khóa học
        /// </summary>
        [HttpPost("{id}/hide")]
        [Authorize(Roles = "GIANGVIEN")]
        public async Task<IActionResult> HideCourse(int id)
        {
            _logger.LogInformation($"Hiding course: {id}");
            try
            {
                var instructorId = GetUserId();
                var result = await _khoaHocService.HideCourseAsync(id, instructorId);
                return Ok(ApiResponse<bool>.SuccessResponse(result, "Ẩn khóa học thành công"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error hiding course {id}");
                if (ex is Middleware.Exceptions.NotFoundException || ex is Middleware.Exceptions.UnauthorizedException)
                {
                    return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
                }
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// [GIANGVIEN] Hiển thị lại khóa học (cần duyệt lại)
        /// </summary>
        [HttpPost("{id}/unhide")]
        [Authorize(Roles = "GIANGVIEN")]
        public async Task<IActionResult> UnhideCourse(int id)
        {
            _logger.LogInformation($"Unhiding course: {id}");
            try
            {
                var instructorId = GetUserId();
                var result = await _khoaHocService.UnhideCourseAsync(id, instructorId);
                return Ok(ApiResponse<bool>.SuccessResponse(result, "Hiển thị lại khóa học thành công."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error unhiding course {id}");
                if (ex is Middleware.Exceptions.NotFoundException || ex is Middleware.Exceptions.UnauthorizedException)
                {
                    return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
                }
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }


        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Không thể xác định người dùng");
            }
            return userId;
        }
    }
}