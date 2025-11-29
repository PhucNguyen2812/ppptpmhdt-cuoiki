using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("NguoiDung")]
[Index("Email", Name = "UQ__NguoiDun__A9D105347B1BB1F3", IsUnique = true)]
public partial class NguoiDung
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string HoTen { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string MatKhau { get; set; } = null!;

    [StringLength(500)]
    public string? AnhDaiDien { get; set; }

    [StringLength(20)]
    public string? SoDienThoai { get; set; }

    [StringLength(500)]
    public string? MoTaNgan { get; set; }

    public string? TieuSu { get; set; }

    [StringLength(200)]
    public string? ChuyenMon { get; set; }

    public bool? TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [InverseProperty("IdGiangVienNavigation")]
    public virtual ICollection<ChiTietChiaSeDoanhThu> ChiTietChiaSeDoanhThus { get; set; } = new List<ChiTietChiaSeDoanhThu>();

    [InverseProperty("IdHocVienNavigation")]
    public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; } = new List<DangKyKhoaHoc>();

    [InverseProperty("IdHocVienNavigation")]
    public virtual ICollection<DanhGiaKhoaHoc> DanhGiaKhoaHocs { get; set; } = new List<DanhGiaKhoaHoc>();

    [InverseProperty("IdNguoiDungNavigation")]
    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    [InverseProperty("IdNguoiDungNavigation")]
    public virtual GioHang? GioHang { get; set; }

    [InverseProperty("IdGiangVienNavigation")]
    public virtual ICollection<KhoaHoc> KhoaHocs { get; set; } = new List<KhoaHoc>();

    [InverseProperty("IdNguoiTaoNavigation")]
    public virtual ICollection<KhuyenMai> KhuyenMais { get; set; } = new List<KhuyenMai>();

    [InverseProperty("IdGiangVienNavigation")]
    public virtual ICollection<LichSuChuyenTien> LichSuChuyenTiens { get; set; } = new List<LichSuChuyenTien>();

    [InverseProperty("IdNguoiDungNavigation")]
    public virtual ICollection<NguoiDungVaiTro> NguoiDungVaiTros { get; set; } = new List<NguoiDungVaiTro>();

    [InverseProperty("IdNguoiDungNavigation")]
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    [InverseProperty("IdHocVienNavigation")]
    public virtual ICollection<TienDoHocTap> TienDoHocTaps { get; set; } = new List<TienDoHocTap>();
}
