using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("NguoiDungVaiTro")]
[Index("IdNguoiDung", "IdVaiTro", Name = "UQ_NguoiDung_VaiTro", IsUnique = true)]
public partial class NguoiDungVaiTro
{
    [Key]
    public int Id { get; set; }

    public int? IdNguoiDung { get; set; }

    public int? IdVaiTro { get; set; }

    [ForeignKey("IdNguoiDung")]
    [InverseProperty("NguoiDungVaiTros")]
    public virtual NguoiDung? IdNguoiDungNavigation { get; set; }

    [ForeignKey("IdVaiTro")]
    [InverseProperty("NguoiDungVaiTros")]
    public virtual VaiTro? IdVaiTroNavigation { get; set; }
}
