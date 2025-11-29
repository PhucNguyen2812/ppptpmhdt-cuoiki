using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("ChiTietChiaSeDoanhThu")]
public partial class ChiTietChiaSeDoanhThu
{
    [Key]
    public int Id { get; set; }

    public int IdDonHang { get; set; }

    public int IdKhoaHoc { get; set; }

    public int IdGiangVien { get; set; }

    public int GiaKhoaHoc { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal TyLeChiaGiangVien { get; set; }

    public int TienGiangVien { get; set; }

    public int TienHeThong { get; set; }

    [StringLength(100)]
    public string? MaGiaoDichChuyenTien { get; set; }

    [ForeignKey("IdDonHang")]
    [InverseProperty("ChiTietChiaSeDoanhThus")]
    public virtual DonHang IdDonHangNavigation { get; set; } = null!;

    [ForeignKey("IdGiangVien")]
    [InverseProperty("ChiTietChiaSeDoanhThus")]
    public virtual NguoiDung IdGiangVienNavigation { get; set; } = null!;

    [ForeignKey("IdKhoaHoc")]
    [InverseProperty("ChiTietChiaSeDoanhThus")]
    public virtual KhoaHoc IdKhoaHocNavigation { get; set; } = null!;

    [InverseProperty("IdChiTietChiaSeNavigation")]
    public virtual ICollection<LichSuChuyenTien> LichSuChuyenTiens { get; set; } = new List<LichSuChuyenTien>();
}
