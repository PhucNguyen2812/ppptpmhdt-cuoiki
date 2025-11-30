using khoahoconline.Dtos.GioHang;

namespace khoahoconline.Services
{
    public interface IGioHangService
    {
        /// <summary>
        /// Lấy giỏ hàng của người dùng
        /// </summary>
        Task<CartDto> GetCartByUserIdAsync(int userId);

        /// <summary>
        /// Thêm khóa học vào giỏ hàng
        /// </summary>
        Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto);

        /// <summary>
        /// Xóa khóa học khỏi giỏ hàng
        /// </summary>
        Task<CartDto> RemoveFromCartAsync(int userId, int cartItemId);

        /// <summary>
        /// Xóa toàn bộ giỏ hàng
        /// </summary>
        Task ClearCartAsync(int userId);

        /// <summary>
        /// Lấy số lượng khóa học trong giỏ hàng
        /// </summary>
        Task<int> GetCartCountAsync(int userId);
    }
}







