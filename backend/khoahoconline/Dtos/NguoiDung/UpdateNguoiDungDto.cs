using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace khoahoconline.Dtos.NguoiDung
{
    public class UpdateNguoiDungDto
    {
        public string HoTen { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string MatKhau { get; set; } = null!;

        public string? SoDienThoai { get; set; }

        public string? AnhDaiDien { get; set; }

        public string? TieuSu { get; set; }

        public bool? TrangThai { get; set; }
    }
}
