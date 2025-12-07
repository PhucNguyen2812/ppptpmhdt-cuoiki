using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace khoahoconline.Dtos.NguoiDung
{
    public class UpdateNguoiDungDto
    {
        public string HoTen { get; set; } = null!;

        public string Email { get; set; } = null!;

        /// <summary>
        /// Mật khẩu mới. Nếu null hoặc rỗng, mật khẩu hiện tại sẽ được giữ nguyên.
        /// </summary>
        public string? MatKhau { get; set; }

        public string? SoDienThoai { get; set; }

        public string? AnhDaiDien { get; set; }

        public string? TieuSu { get; set; }

        public bool? TrangThai { get; set; }

        /// <summary>
        /// Danh sách tên vai trò (roles) cần gán cho người dùng
        /// Không được chứa "ADMIN"
        /// Nếu có "GIANGVIEN" thì tự động thêm "HOCVIEN"
        /// </summary>
        public List<string>? Roles { get; set; }
    }
}
