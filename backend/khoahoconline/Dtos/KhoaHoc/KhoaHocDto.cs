namespace khoahoconline.Dtos.KhoaHoc
{
    /// <summary>
    /// DTO hiển thị thông tin khóa học trong danh sách
    /// </summary>
    public class KhoaHocDto
    {
        public int Id { get; set; }

        public string? TenKhoaHoc { get; set; }

        public string? MoTaNgan { get; set; }

        public decimal? GiaBan { get; set; }

        public string? HinhDaiDien { get; set; }

        public string? MucDo { get; set; }

        public bool? TrangThai { get; set; }

        public int? SoLuongHocVien { get; set; }

        public decimal? DiemDanhGia { get; set; }

        public int? SoLuongDanhGia { get; set; }

        // Thông tin danh mục
        public int? IdDanhMuc { get; set; }
        public string? TenDanhMuc { get; set; }

        // Thông tin giảng viên
        public int? IdGiangVien { get; set; }
        public string? TenGiangVien { get; set; }
        public string? AnhDaiDienGiangVien { get; set; }
    }
}