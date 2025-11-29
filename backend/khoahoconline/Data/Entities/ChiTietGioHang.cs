using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("ChiTietGioHang")]
[Index("IdGioHang", "IdKhoaHoc", Name = "UQ_ChiTietGioHang", IsUnique = true)]
public partial class ChiTietGioHang
{
    [Key]
    public int Id { get; set; }

    public int IdGioHang { get; set; }

    public int IdKhoaHoc { get; set; }

    [ForeignKey("IdGioHang")]
    [InverseProperty("ChiTietGioHangs")]
    public virtual GioHang IdGioHangNavigation { get; set; } = null!;

    [ForeignKey("IdKhoaHoc")]
    [InverseProperty("ChiTietGioHangs")]
    public virtual KhoaHoc IdKhoaHocNavigation { get; set; } = null!;
}
