using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("KhoaHoc")]
public partial class KhoaHoc
{
    [Key]
    public int Id { get; set; }

    public int IdGiangVien { get; set; }

    public int IdDanhMuc { get; set; }

    [StringLength(200)]
    public string? TieuDe { get; set; }

    [StringLength(500)]
    public string? MoTa { get; set; }

    public string? YeuCauTruocKhoaHoc { get; set; }

    public string? HocDuocGi { get; set; }

    [StringLength(500)]
    public string? AnhBia { get; set; }

    [StringLength(500)]
    public string? VideoGioiThieu { get; set; }

    [StringLength(20)]
    public string MucDoKhoaHoc { get; set; } = null!;

    public int? GiaGoc { get; set; }

    public int? SoHocVien { get; set; }

    [Column(TypeName = "decimal(3, 1)")]
    public decimal? DiemDanhGia { get; set; }

    public int? SoLuongDanhGia { get; set; }

    public bool? TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<ChiTietChiaSeDoanhThu> ChiTietChiaSeDoanhThus { get; set; } = new List<ChiTietChiaSeDoanhThu>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<Chuong> Chuongs { get; set; } = new List<Chuong>();

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
    public virtual ICollection<KhoaHocKhuyenMai> KhoaHocKhuyenMais { get; set; } = new List<KhoaHocKhuyenMai>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<TienDoHocTap> TienDoHocTaps { get; set; } = new List<TienDoHocTap>();
}
