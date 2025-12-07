using khoahoconline.Dtos.KhoaHoc;

namespace khoahoconline.Dtos.KiemDuyetKhoaHoc;

public class KiemDuyetKhoaHocDetailDto
{
    public int Id { get; set; }
    public int IdKhoaHoc { get; set; }
    public KhoaHocDetailDto? KhoaHoc { get; set; }
    public int IdNguoiGui { get; set; }
    public string? TenGiangVien { get; set; }
    public string? EmailGiangVien { get; set; }
    public string TrangThai { get; set; } = null!;
    public string? LyDoTuChoi { get; set; }
    public DateTime NgayGui { get; set; }
    public DateTime? NgayDuyet { get; set; }
    public int? IdNguoiDuyet { get; set; }
    public string? TenNguoiDuyet { get; set; }
    public string? GhiChu { get; set; }
}








