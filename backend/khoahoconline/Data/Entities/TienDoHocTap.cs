using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("TienDoHocTap")]
[Index("IdDangKyKhoaHoc", Name = "UQ__TienDoHo__643CEA720053A6C8", IsUnique = true)]
public partial class TienDoHocTap
{
    [Key]
    public int Id { get; set; }

    public int IdDangKyKhoaHoc { get; set; }

    public int IdKhoaHoc { get; set; }

    public int IdHocVien { get; set; }

    public int? SoBaiHocDaHoanThanh { get; set; }

    public int TongSoBaiHoc { get; set; }

    [Column(TypeName = "decimal(5, 1)")]
    public decimal? PhanTramHoanThanh { get; set; }

    [ForeignKey("IdDangKyKhoaHoc")]
    [InverseProperty("TienDoHocTap")]
    public virtual DangKyKhoaHoc IdDangKyKhoaHocNavigation { get; set; } = null!;

    [ForeignKey("IdHocVien")]
    [InverseProperty("TienDoHocTaps")]
    public virtual NguoiDung IdHocVienNavigation { get; set; } = null!;

    [ForeignKey("IdKhoaHoc")]
    [InverseProperty("TienDoHocTaps")]
    public virtual KhoaHoc IdKhoaHocNavigation { get; set; } = null!;

    [InverseProperty("IdTienDoHocTapNavigation")]
    public virtual ICollection<TienDoHocTapChiTiet> TienDoHocTapChiTiets { get; set; } = new List<TienDoHocTapChiTiet>();
}
