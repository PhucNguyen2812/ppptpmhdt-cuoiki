using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("GioHang")]
[Index("IdNguoiDung", Name = "UQ__GioHang__FEE82D41A1A93C8C", IsUnique = true)]
public partial class GioHang
{
    [Key]
    public int Id { get; set; }

    public int IdNguoiDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [InverseProperty("IdGioHangNavigation")]
    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

    [ForeignKey("IdNguoiDung")]
    [InverseProperty("GioHang")]
    public virtual NguoiDung IdNguoiDungNavigation { get; set; } = null!;
}
