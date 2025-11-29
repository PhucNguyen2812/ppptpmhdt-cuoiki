using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("LichSuChuyenTien")]
public partial class LichSuChuyenTien
{
    [Key]
    public int Id { get; set; }

    public int IdChiTietChiaSe { get; set; }

    public int IdGiangVien { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SoTien { get; set; }

    [StringLength(15)]
    public string SoDienThoaiNhan { get; set; } = null!;

    [StringLength(100)]
    public string? MaGiaoDich { get; set; }

    [StringLength(50)]
    public string TrangThai { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? NgayChuyenTien { get; set; }

    [StringLength(500)]
    public string? GhiChu { get; set; }

    [ForeignKey("IdChiTietChiaSe")]
    [InverseProperty("LichSuChuyenTiens")]
    public virtual ChiTietChiaSeDoanhThu IdChiTietChiaSeNavigation { get; set; } = null!;

    [ForeignKey("IdGiangVien")]
    [InverseProperty("LichSuChuyenTiens")]
    public virtual NguoiDung IdGiangVienNavigation { get; set; } = null!;
}
