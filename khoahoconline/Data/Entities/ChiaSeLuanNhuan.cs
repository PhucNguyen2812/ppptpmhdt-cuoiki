using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("ChiaSeLuanNhuan")]
public partial class ChiaSeLuanNhuan
{
    [Key]
    public int Id { get; set; }

    public int IdDonHang { get; set; }

    public int IdKhoaHoc { get; set; }

    public int IdGiangVien { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal DoanhThu { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal TyLeChiaGiangVien { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TienGiangVien { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TienHeThong { get; set; }

    public bool? TrangThaiThanhToan { get; set; }

    [ForeignKey("IdDonHang")]
    [InverseProperty("ChiaSeLuanNhuans")]
    public virtual DonHang IdDonHangNavigation { get; set; } = null!;

    [ForeignKey("IdGiangVien")]
    [InverseProperty("ChiaSeLuanNhuans")]
    public virtual NguoiDung IdGiangVienNavigation { get; set; } = null!;

    [ForeignKey("IdKhoaHoc")]
    [InverseProperty("ChiaSeLuanNhuans")]
    public virtual KhoaHoc IdKhoaHocNavigation { get; set; } = null!;
}
