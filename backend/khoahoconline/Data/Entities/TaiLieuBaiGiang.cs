using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("TaiLieuBaiGiang")]
public partial class TaiLieuBaiGiang
{
    [Key]
    public int Id { get; set; }

    public int IdBaiGiang { get; set; }

    [StringLength(200)]
    public string TenTaiLieu { get; set; } = null!;

    [StringLength(50)]
    public string? LoaiTaiLieu { get; set; }

    [StringLength(500)]
    public string DuongDan { get; set; } = null!;

    public long? KichThuoc { get; set; }

    [StringLength(500)]
    public string? MoTa { get; set; }

    public int? ThuTu { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [ForeignKey("IdBaiGiang")]
    [InverseProperty("TaiLieuBaiGiangs")]
    public virtual BaiGiang IdBaiGiangNavigation { get; set; } = null!;
}
