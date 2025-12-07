using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("Notification")]
public partial class Notification
{
    [Key]
    public int Id { get; set; }

    public int IdNguoiDung { get; set; }

    [StringLength(255)]
    public string TieuDe { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    [StringLength(50)]
    public string Loai { get; set; } = null!;

    [StringLength(50)]
    public string TrangThai { get; set; } = "Chưa đọc";

    public int? IdKhoaHoc { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDoc { get; set; }

    [ForeignKey("IdNguoiDung")]
    [InverseProperty("Notifications")]
    public virtual NguoiDung IdNguoiDungNavigation { get; set; } = null!;

    [ForeignKey("IdKhoaHoc")]
    [InverseProperty("Notifications")]
    public virtual KhoaHoc? IdKhoaHocNavigation { get; set; }
}








