using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("DangKyKhoaHoc")]
[Index("IdHocVien", Name = "IX_DangKyKhoaHoc_HocVien")]
[Index("IdHocVien", "IdKhoaHoc", Name = "UQ_HocVien_KhoaHoc", IsUnique = true)]
public partial class DangKyKhoaHoc
{
    [Key]
    public int Id { get; set; }

    public int? IdHocVien { get; set; }

    public int? IdKhoaHoc { get; set; }

    public int? IdDonHang { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDangKy { get; set; }

    [ForeignKey("IdDonHang")]
    [InverseProperty("DangKyKhoaHocs")]
    public virtual DonHang? IdDonHangNavigation { get; set; }

    [ForeignKey("IdHocVien")]
    [InverseProperty("DangKyKhoaHocs")]
    public virtual NguoiDung? IdHocVienNavigation { get; set; }

    [ForeignKey("IdKhoaHoc")]
    [InverseProperty("DangKyKhoaHocs")]
    public virtual KhoaHoc? IdKhoaHocNavigation { get; set; }

    [InverseProperty("IdDangKyNavigation")]
    public virtual ICollection<TienDoHocTap> TienDoHocTaps { get; set; } = new List<TienDoHocTap>();
}
