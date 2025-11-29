using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("DonHang")]
public partial class DonHang
{
    [Key]
    public int Id { get; set; }

    public int IdNguoiDung { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TongTien { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TienGiam { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TongTienThanhToan { get; set; }

    [StringLength(50)]
    public string TrangThaiDonHang { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? NgayThanhToan { get; set; }

    [StringLength(500)]
    public string? GhiChu { get; set; }

    [InverseProperty("IdDonHangNavigation")]
    public virtual ICollection<ChiTietChiaSeDoanhThu> ChiTietChiaSeDoanhThus { get; set; } = new List<ChiTietChiaSeDoanhThu>();

    [InverseProperty("IdDonHangNavigation")]
    public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; } = new List<DangKyKhoaHoc>();

    [ForeignKey("IdNguoiDung")]
    [InverseProperty("DonHangs")]
    public virtual NguoiDung IdNguoiDungNavigation { get; set; } = null!;
}
