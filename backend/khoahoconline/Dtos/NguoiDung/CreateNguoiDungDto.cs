using System.ComponentModel.DataAnnotations;

namespace khoahoconline.Dtos.NguoiDung
{
    public class CreateNguoiDungDto
    {
        public string HoTen { get; set; } = null!;

        [StringLength(100)]
        public string Email { get; set; } = null!;

        [StringLength(255)]
        public string MatKhau { get; set; } = null!;

        [StringLength(15)]
        public string? SoDienThoai { get; set; }

        [StringLength(500)]
        public string? AnhDaiDien { get; set; }

        public string? TieuSu { get; set; }

    }
}
