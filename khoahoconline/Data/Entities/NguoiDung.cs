using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("NguoiDung")]
[Index("Email", Name = "IX_NguoiDung_Email")]
[Index("Email", Name = "UQ__NguoiDun__A9D105344A4D3C7C", IsUnique = true)]
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

    [StringLength(15)]
    public string? SoDienThoai { get; set; }

    [StringLength(500)]
    public string? AnhDaiDien { get; set; }

    public string? TieuSu { get; set; }

    public bool? TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayCapNhat { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LanDangNhapCuoi { get; set; }

    [InverseProperty("IdGiangVienNavigation")]
    public virtual ICollection<ChiaSeLuanNhuan> ChiaSeLuanNhuans { get; set; } = new List<ChiaSeLuanNhuan>();

    [InverseProperty("IdHocVienNavigation")]
    public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; } = new List<DangKyKhoaHoc>();

    [InverseProperty("IdHocVienNavigation")]
    public virtual ICollection<DanhGiaKhoaHoc> DanhGiaKhoaHocs { get; set; } = new List<DanhGiaKhoaHoc>();

    [InverseProperty("IdHocVienNavigation")]
    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    [InverseProperty("IdHocVienNavigation")]
    public virtual GioHang? GioHang { get; set; }

    [InverseProperty("IdGiangVienNavigation")]
    public virtual ICollection<KhoaHoc> KhoaHocs { get; set; } = new List<KhoaHoc>();

    [InverseProperty("IdNguoiDungNavigation")]
    public virtual ICollection<NguoiDungVaiTro> NguoiDungVaiTros { get; set; } = new List<NguoiDungVaiTro>();

    [InverseProperty("IdNguoiDungNavigation")]
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
