using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("TaiLieuKhoaHoc")]
public partial class TaiLieuKhoaHoc
{
    [Key]
    public int Id { get; set; }

    public int? IdKhoaHoc { get; set; }

    [StringLength(255)]
    public string? TenTaiLieu { get; set; }

    [StringLength(500)]
    public string? MoTa { get; set; }

    [StringLength(500)]
    public string? DuongDan { get; set; }

    [StringLength(50)]
    public string? LoaiTaiLieu { get; set; }

    public long? KichThuoc { get; set; }

    public int? ThuTu { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [ForeignKey("IdKhoaHoc")]
    [InverseProperty("TaiLieuKhoaHocs")]
    public virtual KhoaHoc? IdKhoaHocNavigation { get; set; }
}
