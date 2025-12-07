using khoahoconline.Data.Entities;

namespace khoahoconline.Data.Repositories
{
    public interface IGioHangRepository : IBaseRepository<GioHang>
    {
        /// <summary>
        /// Lấy giỏ hàng của người dùng kèm chi tiết
        /// </summary>
        Task<GioHang?> GetByUserIdAsync(int userId);

        /// <summary>
        /// Tạo giỏ hàng mới cho người dùng
        /// </summary>
        Task<GioHang> CreateCartForUserAsync(int userId);

        /// <summary>
        /// Lấy chi tiết giỏ hàng theo ID
        /// </summary>
        Task<ChiTietGioHang?> GetCartItemByIdAsync(int id);

        /// <summary>
        /// Lấy chi tiết giỏ hàng theo giỏ hàng và khóa học
        /// </summary>
        Task<ChiTietGioHang?> GetCartItemByCartAndCourseAsync(int cartId, int courseId);

        /// <summary>
        /// Thêm item vào giỏ hàng
        /// </summary>
        Task<ChiTietGioHang> AddItemAsync(ChiTietGioHang item);

        /// <summary>
        /// Xóa item khỏi giỏ hàng
        /// </summary>
        Task RemoveItemAsync(ChiTietGioHang item);

        /// <summary>
        /// Xóa tất cả items trong giỏ hàng
        /// </summary>
        Task ClearCartAsync(int cartId);

        /// <summary>
        /// Cập nhật tổng tiền giỏ hàng
        /// </summary>
        Task UpdateCartTotalAsync(int cartId);
    }
}
















