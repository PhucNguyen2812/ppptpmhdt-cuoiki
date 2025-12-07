namespace khoahoconline.Dtos.InstructorRequest;

public class InstructorRequestDto
{
    public int Id { get; set; }
    public int IdHocVien { get; set; }
    public string? HoTen { get; set; }
    public string? Email { get; set; }
    public string ChungChiPath { get; set; } = null!;
    public string? ThongTinBoSung { get; set; }
    public string TrangThai { get; set; } = null!;
    public string? LyDoTuChoi { get; set; }
    public DateTime NgayGui { get; set; }
    public DateTime? NgayDuyet { get; set; }
    public int? IdNguoiDuyet { get; set; }
    public string? TenNguoiDuyet { get; set; }
}








