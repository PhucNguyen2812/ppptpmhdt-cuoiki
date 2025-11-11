using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace khoahoconline.Dtos.NguoiDung
{
    public class NguoiDungDto
    {
        public int Id { get; set; }

        public string HoTen { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? SoDienThoai { get; set; }

        public string? AnhDaiDien { get; set; }

        public string? TieuSu { get; set; }

        public bool? TrangThai { get; set; }

        public DateTime? NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }
    }
}
