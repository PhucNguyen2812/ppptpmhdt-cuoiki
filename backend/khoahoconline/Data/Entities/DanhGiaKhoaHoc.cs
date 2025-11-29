using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("DanhGiaKhoaHoc")]
[Index("IdKhoaHoc", "IdHocVien", Name = "UQ_DanhGiaKhoaHoc", IsUnique = true)]
public partial class DanhGiaKhoaHoc
{
    [Key]
    public int Id { get; set; }

    public int IdKhoaHoc { get; set; }

    public int IdHocVien { get; set; }

    public int DiemDanhGia { get; set; }

    public string? NoiDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDanhGia { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [ForeignKey("IdHocVien")]
    [InverseProperty("DanhGiaKhoaHocs")]
    public virtual NguoiDung IdHocVienNavigation { get; set; } = null!;

    [ForeignKey("IdKhoaHoc")]
    [InverseProperty("DanhGiaKhoaHocs")]
    public virtual KhoaHoc IdKhoaHocNavigation { get; set; } = null!;
}
