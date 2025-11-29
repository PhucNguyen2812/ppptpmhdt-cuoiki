namespace khoahoconline.Dtos.NguoiDung
{
    public class NguoiDungDetailDto
    {
        public int Id { get; set; }
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? SoDienThoai { get; set; }
        public string? AnhDaiDien { get; set; }
        public string? MoTaNgan { get; set; }
        public string? TieuSu { get; set; }
        public string? ChuyenMon { get; set; }
        public bool? TrangThai { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        
        // Danh sách vai trò
        public List<string> VaiTros { get; set; } = new List<string>();
    }
}