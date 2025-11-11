using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("GioHang")]
[Index("IdHocVien", Name = "UQ_GioHang_HocVien", IsUnique = true)]
public partial class GioHang
{
    [Key]
    public int Id { get; set; }

    public int IdHocVien { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TongTienGoc { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? PhanTramGiam { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TienGiamVoucher { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TongTienThanhToan { get; set; }

    public int? SoLuongKhoaHoc { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [InverseProperty("IdGioHangNavigation")]
    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

    [ForeignKey("IdHocVien")]
    [InverseProperty("GioHang")]
    public virtual NguoiDung IdHocVienNavigation { get; set; } = null!;
}
