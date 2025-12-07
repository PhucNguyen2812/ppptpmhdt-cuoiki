using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using khoahoconline.Dtos;
using khoahoconline.Dtos.GioHang;
using khoahoconline.Services;

namespace khoahoconline.Controllers
{
    [Route("api/v1/cart")]
    [ApiController]
    [Authorize]
    public class GioHangController : ControllerBase
    {
        private readonly ILogger<GioHangController> _logger;
        private readonly IGioHangService _gioHangService;

        public GioHangController(
            ILogger<GioHangController> logger,
            IGioHangService gioHangService)
        {
            _logger = logger;
            _gioHangService = gioHangService;
        }

        /// <summary>
        /// [AUTH] Lấy giỏ hàng của người dùng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var result = await _gioHangService.GetCartByUserIdAsync(userId);
            return Ok(ApiResponse<CartDto>.SuccessResponse(result));
        }

        /// <summary>
        /// [AUTH] Lấy số lượng khóa học trong giỏ hàng
        /// </summary>
        [HttpGet("count")]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = GetUserId();
            var count = await _gioHangService.GetCartCountAsync(userId);
            return Ok(ApiResponse<int>.SuccessResponse(count));
        }

        /// <summary>
        /// [AUTH] Thêm khóa học vào giỏ hàng
        /// </summary>
        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var userId = GetUserId();
            var result = await _gioHangService.AddToCartAsync(userId, dto);
            return Ok(ApiResponse<CartDto>.SuccessResponse(result, "Đã thêm khóa học vào giỏ hàng"));
        }

        /// <summary>
        /// [AUTH] Xóa khóa học khỏi giỏ hàng
        /// </summary>
        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = GetUserId();
            var result = await _gioHangService.RemoveFromCartAsync(userId, cartItemId);
            return Ok(ApiResponse<CartDto>.SuccessResponse(result, "Đã xóa khóa học khỏi giỏ hàng"));
        }

        /// <summary>
        /// [AUTH] Xóa toàn bộ giỏ hàng
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            await _gioHangService.ClearCartAsync(userId);
            return Ok(ApiResponse<string>.SuccessResponse("Đã xóa toàn bộ giỏ hàng"));
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
















