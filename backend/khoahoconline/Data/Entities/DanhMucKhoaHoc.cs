using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("DanhMucKhoaHoc")]
[Index("TenDanhMuc", Name = "UQ__DanhMucK__650CAE4E0512495F", IsUnique = true)]
public partial class DanhMucKhoaHoc
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string TenDanhMuc { get; set; } = null!;

    [StringLength(500)]
    public string? MoTa { get; set; }

    public bool? TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [InverseProperty("IdDanhMucNavigation")]
    public virtual ICollection<KhoaHoc> KhoaHocs { get; set; } = new List<KhoaHoc>();
}
