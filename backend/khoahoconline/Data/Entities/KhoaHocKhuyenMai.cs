using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("KhoaHocKhuyenMai")]
[Index("IdKhoaHoc", "IdKhuyenMai", Name = "UQ_KhoaHocKhuyenMai", IsUnique = true)]
public partial class KhoaHocKhuyenMai
{
    [Key]
    public int Id { get; set; }

    public int? IdKhoaHoc { get; set; }

    public int? IdKhuyenMai { get; set; }

    [StringLength(50)]
    public string MaKhuyenMai { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal GiaGoc { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal GiaSauKhuyenMai { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal PhanTramGiamThucTe { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SoTienGiam { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayBatDau { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayKetThuc { get; set; }

    public int? SoLuotDaSuDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [ForeignKey("IdKhoaHoc")]
    [InverseProperty("KhoaHocKhuyenMais")]
    public virtual KhoaHoc? IdKhoaHocNavigation { get; set; }

    [ForeignKey("IdKhuyenMai")]
    [InverseProperty("KhoaHocKhuyenMais")]
    public virtual KhuyenMai? IdKhuyenMaiNavigation { get; set; }
}
