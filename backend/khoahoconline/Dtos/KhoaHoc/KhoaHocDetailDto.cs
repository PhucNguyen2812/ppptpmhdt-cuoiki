using khoahoconline.Dtos.DanhMuc;
using khoahoconline.Dtos.NguoiDung;

namespace khoahoconline.Dtos.KhoaHoc
{
    /// <summary>
    /// DTO hiển thị thông tin chi tiết khóa học
    /// </summary>
    public class KhoaHocDetailDto
    {
        public int Id { get; set; }

        public string? TenKhoaHoc { get; set; }

        public string? MoTaNgan { get; set; }

        public string? MoTaChiTiet { get; set; }

        public string? YeuCauTruoc { get; set; }

        public string? HocDuoc { get; set; }

        public decimal? GiaBan { get; set; }

        public string? HinhDaiDien { get; set; }

        public string? VideoGioiThieu { get; set; }

        public string? MucDo { get; set; }

        public bool? TrangThai { get; set; }

        public int? SoLuongHocVien { get; set; }

        public decimal? DiemDanhGia { get; set; }

        public int? SoLuongDanhGia { get; set; }

        // Thông tin danh mục (nested object)
        public DanhMucDto? DanhMuc { get; set; }

        // Thông tin giảng viên (nested object)
        public NguoiDungDto? GiangVien { get; set; }
    }
}