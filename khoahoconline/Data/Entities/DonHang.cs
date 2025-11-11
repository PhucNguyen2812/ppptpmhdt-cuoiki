using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("DonHang")]
[Index("IdHocVien", Name = "IX_DonHang_HocVien")]
public partial class DonHang
{
    [Key]
    public int Id { get; set; }

    public int? IdHocVien { get; set; }

    public int? IdVoucher { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TongTienGoc { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TienGiam { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TongTienThanhToan { get; set; }

    [StringLength(50)]
    public string? PhuongThucThanhToan { get; set; }

    [StringLength(50)]
    public string TrangThaiThanhToan { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? NgayThanhToan { get; set; }

    [StringLength(500)]
    public string? GhiChu { get; set; }

    [InverseProperty("IdDonHangNavigation")]
    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    [InverseProperty("IdDonHangNavigation")]
    public virtual ICollection<ChiaSeLuanNhuan> ChiaSeLuanNhuans { get; set; } = new List<ChiaSeLuanNhuan>();

    [InverseProperty("IdDonHangNavigation")]
    public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; } = new List<DangKyKhoaHoc>();

    [ForeignKey("IdHocVien")]
    [InverseProperty("DonHangs")]
    public virtual NguoiDung? IdHocVienNavigation { get; set; }

    [ForeignKey("IdVoucher")]
    [InverseProperty("DonHangs")]
    public virtual Voucher? IdVoucherNavigation { get; set; }
}
