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

    public int IdChuong { get; set; }

    [StringLength(200)]
    public string TieuDe { get; set; } = null!;

    public string? MoTa { get; set; }

    [StringLength(500)]
    public string? DuongDanVideo { get; set; }

    public int ThuTu { get; set; }

    public bool? MienPhiXem { get; set; }

    public bool? TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [ForeignKey("IdChuong")]
    [InverseProperty("BaiGiangs")]
    public virtual Chuong IdChuongNavigation { get; set; } = null!;

    [InverseProperty("IdBaiGiangNavigation")]
    public virtual ICollection<TaiLieuBaiGiang> TaiLieuBaiGiangs { get; set; } = new List<TaiLieuBaiGiang>();

    [InverseProperty("IdBaiGiangNavigation")]
    public virtual ICollection<TienDoHocTapChiTiet> TienDoHocTapChiTiets { get; set; } = new List<TienDoHocTapChiTiet>();
}
