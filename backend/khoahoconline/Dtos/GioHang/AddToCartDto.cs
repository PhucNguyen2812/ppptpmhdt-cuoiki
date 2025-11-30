using System.ComponentModel.DataAnnotations;

namespace khoahoconline.Dtos.GioHang
{
    /// <summary>
    /// DTO để thêm khóa học vào giỏ hàng
    /// </summary>
    public class AddToCartDto
    {
        [Required(ErrorMessage = "Id khóa học là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Id khóa học phải lớn hơn 0")]
        public int IdKhoaHoc { get; set; }
    }
}







