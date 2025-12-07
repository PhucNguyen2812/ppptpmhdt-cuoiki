using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using khoahoconline.Dtos;
using khoahoconline.Dtos.Review;
using khoahoconline.Services;

namespace khoahoconline.Controllers
{
    [Route("api/v1/reviews")]
    [ApiController]
    public class DanhGiaController : ControllerBase
    {
        private readonly ILogger<DanhGiaController> _logger;
        private readonly IDanhGiaService _danhGiaService;

        public DanhGiaController(
            ILogger<DanhGiaController> logger,
            IDanhGiaService danhGiaService)
        {
            _logger = logger;
            _danhGiaService = danhGiaService;
        }

        /// <summary>
        /// [AUTH] Tạo đánh giá mới cho khóa học
        /// </summary>
        /// <param name="dto">Thông tin đánh giá</param>
        /// <returns>Đánh giá đã tạo</returns>
        [HttpPost]
        [Authorize(Roles = "HOCVIEN")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            _logger.LogInformation("Creating new review");

            try
            {
                var userId = GetUserId();
                var result = await _danhGiaService.CreateReviewAsync(userId, dto);
                return Ok(ApiResponse<ReviewDto>.SuccessResponse(result, "Đánh giá thành công"));
            }
            catch (Middleware.Exceptions.BadRequestException ex)
            {
                _logger.LogWarning(ex, "Bad request when creating review");
                return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Middleware.Exceptions.NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found when creating review");
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review");
                return BadRequest(ApiResponse<string>.ErrorResponse($"Lỗi khi tạo đánh giá: {ex.Message}"));
            }
        }

        /// <summary>
        /// [AUTH] Cập nhật đánh giá của mình
        /// </summary>
        /// <param name="id">ID đánh giá</param>
        /// <param name="dto">Thông tin đánh giá cập nhật</param>
        /// <returns>Đánh giá đã cập nhật</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "HOCVIEN")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewDto dto)
        {
            _logger.LogInformation($"Updating review {id}");

            try
            {
                var userId = GetUserId();
                var result = await _danhGiaService.UpdateReviewAsync(id, userId, dto);
                return Ok(ApiResponse<ReviewDto>.SuccessResponse(result, "Cập nhật đánh giá thành công"));
            }
            catch (Middleware.Exceptions.NotFoundException ex)
            {
                _logger.LogWarning(ex, $"Review {id} not found");
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Middleware.Exceptions.UnauthorizedException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized access to review {id}");
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating review {id}");
                return BadRequest(ApiResponse<string>.ErrorResponse($"Lỗi khi cập nhật đánh giá: {ex.Message}"));
            }
        }

        /// <summary>
        /// [AUTH] Xóa đánh giá của mình
        /// </summary>
        /// <param name="id">ID đánh giá</param>
        /// <returns>Kết quả</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "HOCVIEN")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            _logger.LogInformation($"Deleting review {id}");

            try
            {
                var userId = GetUserId();
                var result = await _danhGiaService.DeleteReviewAsync(id, userId);
                return Ok(ApiResponse<bool>.SuccessResponse(result, "Xóa đánh giá thành công"));
            }
            catch (Middleware.Exceptions.NotFoundException ex)
            {
                _logger.LogWarning(ex, $"Review {id} not found");
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Middleware.Exceptions.UnauthorizedException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized access to review {id}");
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting review {id}");
                return BadRequest(ApiResponse<string>.ErrorResponse($"Lỗi khi xóa đánh giá: {ex.Message}"));
            }
        }

        /// <summary>
        /// [AUTH] Lấy đánh giá của mình cho một khóa học
        /// </summary>
        /// <param name="courseId">ID khóa học</param>
        /// <returns>Đánh giá của mình (null nếu chưa đánh giá)</returns>
        [HttpGet("my-review/{courseId}")]
        [Authorize(Roles = "HOCVIEN")]
        public async Task<IActionResult> GetMyReview(int courseId)
        {
            _logger.LogInformation($"Getting my review for course {courseId}");

            try
            {
                var userId = GetUserId();
                var result = await _danhGiaService.GetMyReviewAsync(courseId, userId);
                
                if (result == null)
                {
                    return Ok(ApiResponse<ReviewDto?>.SuccessResponse(null, "Bạn chưa đánh giá khóa học này"));
                }
                
                return Ok(ApiResponse<ReviewDto>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting my review for course {courseId}");
                return BadRequest(ApiResponse<string>.ErrorResponse($"Lỗi khi lấy đánh giá: {ex.Message}"));
            }
        }

        /// <summary>
        /// [PUBLIC] Lấy danh sách đánh giá của khóa học
        /// </summary>
        /// <param name="courseId">ID khóa học</param>
        /// <param name="diemDanhGia">Lọc theo điểm đánh giá (1-5, optional)</param>
        /// <param name="pageNumber">Số trang (default: 1)</param>
        /// <param name="pageSize">Số items mỗi trang (default: 10)</param>
        /// <returns>Danh sách đánh giá với pagination</returns>
        [HttpGet("course/{courseId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByCourseId(
            int courseId,
            [FromQuery] int? diemDanhGia = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation($"Getting reviews for course {courseId}");

            try
            {
                var filter = new ReviewFilterDto
                {
                    IdKhoaHoc = courseId,
                    DiemDanhGia = diemDanhGia,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _danhGiaService.GetReviewsByCourseIdAsync(courseId, filter);
                return Ok(ApiResponse<PagedResult<ReviewDto>>.SuccessResponse(result));
            }
            catch (Middleware.Exceptions.NotFoundException ex)
            {
                _logger.LogWarning(ex, $"Course {courseId} not found");
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting reviews for course {courseId}");
                return BadRequest(ApiResponse<string>.ErrorResponse($"Lỗi khi lấy danh sách đánh giá: {ex.Message}"));
            }
        }

        /// <summary>
        /// [PUBLIC] Lấy tổng hợp đánh giá của khóa học (điểm trung bình, số lượng đánh giá, phân bố điểm)
        /// </summary>
        /// <param name="courseId">ID khóa học</param>
        /// <returns>Tổng hợp đánh giá</returns>
        [HttpGet("course/{courseId}/summary")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourseReviewSummary(int courseId)
        {
            _logger.LogInformation($"Getting review summary for course {courseId}");

            try
            {
                var result = await _danhGiaService.GetCourseReviewSummaryAsync(courseId);
                return Ok(ApiResponse<CourseReviewSummaryDto>.SuccessResponse(result));
            }
            catch (Middleware.Exceptions.NotFoundException ex)
            {
                _logger.LogWarning(ex, $"Course {courseId} not found");
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting review summary for course {courseId}");
                return BadRequest(ApiResponse<string>.ErrorResponse($"Lỗi khi lấy tổng hợp đánh giá: {ex.Message}"));
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




