using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("KhoaHoc")]
[Index("IdDanhMuc", Name = "IX_KhoaHoc_DanhMuc")]
[Index("IdGiangVien", Name = "IX_KhoaHoc_GiangVien")]
public partial class KhoaHoc
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string TenKhoaHoc { get; set; } = null!;

    [StringLength(500)]
    public string? MoTaNgan { get; set; }

    public string? MoTaChiTiet { get; set; }

    public int IdDanhMuc { get; set; }

    public int IdGiangVien { get; set; }

    public string? YeuCauTruoc { get; set; }

    public string? HocDuoc { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal GiaBan { get; set; }

    [StringLength(500)]
    public string? HinhDaiDien { get; set; }

    [StringLength(500)]
    public string? VideoGioiThieu { get; set; }

    [StringLength(50)]
    public string? MucDo { get; set; }

    public bool? TrangThai { get; set; }

    public int? SoLuongHocVien { get; set; }

    [Column(TypeName = "decimal(3, 2)")]
    public decimal? DiemDanhGia { get; set; }

    public int? SoLuongDanhGia { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<BaiGiang> BaiGiangs { get; set; } = new List<BaiGiang>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<ChiaSeLuanNhuan> ChiaSeLuanNhuans { get; set; } = new List<ChiaSeLuanNhuan>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; } = new List<DangKyKhoaHoc>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<DanhGiaKhoaHoc> DanhGiaKhoaHocs { get; set; } = new List<DanhGiaKhoaHoc>();

    [ForeignKey("IdDanhMuc")]
    [InverseProperty("KhoaHocs")]
    public virtual DanhMucKhoaHoc IdDanhMucNavigation { get; set; } = null!;

    [ForeignKey("IdGiangVien")]
    [InverseProperty("KhoaHocs")]
    public virtual NguoiDung IdGiangVienNavigation { get; set; } = null!;

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<TaiLieuKhoaHoc> TaiLieuKhoaHocs { get; set; } = new List<TaiLieuKhoaHoc>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<TienDoHocTap> TienDoHocTaps { get; set; } = new List<TienDoHocTap>();
}
