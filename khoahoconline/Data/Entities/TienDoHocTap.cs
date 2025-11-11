using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("TienDoHocTap")]
[Index("IdDangKy", "IdKhoaHoc", Name = "UQ_DangKy_KhoaHoc", IsUnique = true)]
public partial class TienDoHocTap
{
    [Key]
    public int Id { get; set; }

    public int IdDangKy { get; set; }

    public int IdKhoaHoc { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? PhanTramHoanThanh { get; set; }

    public bool? DaHoanThanh { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayBatDau { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayHoanThanh { get; set; }

    [ForeignKey("IdDangKy")]
    [InverseProperty("TienDoHocTaps")]
    public virtual DangKyKhoaHoc IdDangKyNavigation { get; set; } = null!;

    [ForeignKey("IdKhoaHoc")]
    [InverseProperty("TienDoHocTaps")]
    public virtual KhoaHoc IdKhoaHocNavigation { get; set; } = null!;
}
