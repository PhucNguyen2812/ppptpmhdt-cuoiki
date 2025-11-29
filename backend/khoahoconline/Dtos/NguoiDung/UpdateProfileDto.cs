using System.ComponentModel.DataAnnotations;

namespace khoahoconline.Dtos.NguoiDung
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string HoTen { get; set; } = null!;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string? SoDienThoai { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả ngắn không được vượt quá 500 ký tự")]
        public string? MoTaNgan { get; set; }

        public string? TieuSu { get; set; }

        [StringLength(200, ErrorMessage = "Chuyên môn không được vượt quá 200 ký tự")]
        public string? ChuyenMon { get; set; }
    }
}