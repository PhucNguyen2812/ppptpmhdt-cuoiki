using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("YeuCauDangKyGiangVien")]
public partial class YeuCauDangKyGiangVien
{
    [Key]
    public int Id { get; set; }

    public int IdHocVien { get; set; }

    [StringLength(500)]
    public string ChungChiPath { get; set; } = null!;

    public string? ThongTinBoSung { get; set; }

    [StringLength(50)]
    public string TrangThai { get; set; } = null!; // Chờ duyệt/Đã duyệt/Từ chối

    [StringLength(500)]
    public string? LyDoTuChoi { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayGui { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDuyet { get; set; }

    public int? IdNguoiDuyet { get; set; }

    [ForeignKey("IdHocVien")]
    [InverseProperty("YeuCauDangKyGiangViens")]
    public virtual NguoiDung IdHocVienNavigation { get; set; } = null!;

    [ForeignKey("IdNguoiDuyet")]
    [InverseProperty("YeuCauDangKyGiangVienDuyets")]
    public virtual NguoiDung? IdNguoiDuyetNavigation { get; set; }
}








