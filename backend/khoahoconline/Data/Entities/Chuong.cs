using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("Chuong")]
public partial class Chuong
{
    [Key]
    public int Id { get; set; }

    public int? IdKhoaHoc { get; set; }

    [StringLength(200)]
    public string? TenChuong { get; set; }

    [StringLength(1000)]
    public string? MucTieu { get; set; }

    public int ThuTu { get; set; }

    public int? SoLuongBaiGiang { get; set; }

    public bool? TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [InverseProperty("IdChuongNavigation")]
    public virtual ICollection<BaiGiang> BaiGiangs { get; set; } = new List<BaiGiang>();

    [ForeignKey("IdKhoaHoc")]
    [InverseProperty("Chuongs")]
    public virtual KhoaHoc? IdKhoaHocNavigation { get; set; }
}
