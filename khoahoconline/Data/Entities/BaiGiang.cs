using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("BaiGiang")]
public partial class BaiGiang
{
    [Key]
    public int Id { get; set; }

    public int IdKhoaHoc { get; set; }

    [StringLength(255)]
    public string TieuDe { get; set; } = null!;

    [StringLength(500)]
    public string? MoTa { get; set; }

    [StringLength(500)]
    public string? VideoUrl { get; set; }

    public int? ThoiLuong { get; set; }

    public int ThuTu { get; set; }

    public bool? XemThuMienPhi { get; set; }

    public bool? TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [ForeignKey("IdKhoaHoc")]
    [InverseProperty("BaiGiangs")]
    public virtual KhoaHoc IdKhoaHocNavigation { get; set; } = null!;
}
