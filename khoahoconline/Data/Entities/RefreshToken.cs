using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("RefreshToken")]
public partial class RefreshToken
{
    [Key]
    public int Id { get; set; }

    public int? IdNguoiDung { get; set; }

    [StringLength(500)]
    public string? Token { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayHetHan { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayThuHoi { get; set; }

    [ForeignKey("IdNguoiDung")]
    [InverseProperty("RefreshTokens")]
    public virtual NguoiDung? IdNguoiDungNavigation { get; set; }
}
