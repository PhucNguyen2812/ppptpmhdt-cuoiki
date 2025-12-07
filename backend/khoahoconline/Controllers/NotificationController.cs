using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using khoahoconline.Dtos;
using khoahoconline.Dtos.Notification;
using khoahoconline.Services;

namespace khoahoconline.Controllers
{
    [Route("api/v1/notifications")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationService _notificationService;

        public NotificationController(
            ILogger<NotificationController> logger,
            INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Lấy danh sách thông báo
        /// </summary>
        /// <param name="trangThai">Trạng thái: Chưa đọc/Đã đọc (optional)</param>
        /// <param name="pageNumber">Số trang (default: 1)</param>
        /// <param name="pageSize">Số items mỗi trang (default: 10)</param>
        /// <returns>Danh sách thông báo với pagination</returns>
        [HttpGet]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] string? trangThai = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation($"Getting notifications for user, trangThai: {trangThai}, page: {pageNumber}, size: {pageSize}");

            try
            {
                var userId = GetUserId();
                var result = await _notificationService.GetNotificationsAsync(userId, trangThai, pageNumber, pageSize);
                return Ok(ApiResponse<PagedNotificationResult>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications");
                return BadRequest(ApiResponse<string>.ErrorResponse($"Lỗi khi lấy danh sách thông báo: {ex.Message}"));
            }
        }

        /// <summary>
        /// Lấy số lượng thông báo chưa đọc
        /// </summary>
        /// <returns>Số lượng thông báo chưa đọc</returns>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            _logger.LogInformation("Getting unread notification count");

            try
            {
                var userId = GetUserId();
                var count = await _notificationService.GetUnreadCountAsync(userId);
                return Ok(ApiResponse<int>.SuccessResponse(count, $"Có {count} thông báo chưa đọc"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count");
                return BadRequest(ApiResponse<string>.ErrorResponse($"Lỗi khi lấy số thông báo chưa đọc: {ex.Message}"));
            }
        }

        /// <summary>
        /// Đánh dấu thông báo đã đọc
        /// </summary>
        /// <param name="id">ID thông báo</param>
        /// <returns>Kết quả</returns>
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            _logger.LogInformation($"Marking notification {id} as read");

            try
            {
                var userId = GetUserId();
                await _notificationService.MarkAsReadAsync(id, userId);
                return Ok(ApiResponse<string>.SuccessResponse("Đã đánh dấu đọc"));
            }
            catch (Middleware.Exceptions.NotFoundException ex)
            {
                _logger.LogWarning(ex, $"Notification {id} not found");
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Middleware.Exceptions.UnauthorizedException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized access to notification {id}");
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking notification {id} as read");
                return BadRequest(ApiResponse<string>.ErrorResponse($"Lỗi khi đánh dấu đọc: {ex.Message}"));
            }
        }

        /// <summary>
        /// Đánh dấu tất cả thông báo đã đọc
        /// </summary>
        /// <returns>Kết quả</returns>
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            _logger.LogInformation("Marking all notifications as read");

            try
            {
                var userId = GetUserId();
                await _notificationService.MarkAllAsReadAsync(userId);
                return Ok(ApiResponse<string>.SuccessResponse("Đã đánh dấu tất cả thông báo đã đọc"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                return BadRequest(ApiResponse<string>.ErrorResponse($"Lỗi khi đánh dấu tất cả đã đọc: {ex.Message}"));
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




