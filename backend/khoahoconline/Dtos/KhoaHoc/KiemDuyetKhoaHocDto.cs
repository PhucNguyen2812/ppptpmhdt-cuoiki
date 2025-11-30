namespace khoahoconline.Dtos.KhoaHoc
{
    public class KiemDuyetKhoaHocDto
    {
        public int Id { get; set; }
        public int IdKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = null!;
        public int PhienBan { get; set; }
        public string TrangThaiKiemDuyet { get; set; } = null!;
        public int IdNguoiGui { get; set; }
        public string TenNguoiGui { get; set; } = null!;
        public int? IdNguoiKiemDuyet { get; set; }
        public string? TenNguoiKiemDuyet { get; set; }
        public string? LyDoTuChoi { get; set; }
        public DateTime NgayGui { get; set; }
        public DateTime? NgayKiemDuyet { get; set; }
        public string? NoiDungThayDoi { get; set; }
        public string? GhiChu { get; set; }
    }
}





