using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("KhuyenMai")]
public partial class KhuyenMai
{
    [Key]
    public int Id { get; set; }

    public int? IdNguoiTao { get; set; }

    [StringLength(200)]
    public string? TenKhuyenMai { get; set; }

    [StringLength(500)]
    public string? MoTa { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? PhanTramGiam { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? PhanTramGiamToiThieu { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? PhanTramGiamToiDa { get; set; }

    public bool? GioiHanSoLuot { get; set; }

    public int? SoLuotToiDa { get; set; }

    public bool? TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [ForeignKey("IdNguoiTao")]
    [InverseProperty("KhuyenMais")]
    public virtual NguoiDung? IdNguoiTaoNavigation { get; set; }

    [InverseProperty("IdKhuyenMaiNavigation")]
    public virtual ICollection<KhoaHocKhuyenMai> KhoaHocKhuyenMais { get; set; } = new List<KhoaHocKhuyenMai>();
}
