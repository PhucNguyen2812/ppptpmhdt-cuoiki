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

    [StringLength(255)]
    public string? TenKhoaHoc { get; set; }

    [StringLength(500)]
    public string? MoTaNgan { get; set; }

    public string? MoTaChiTiet { get; set; }

    public int? IdDanhMuc { get; set; }

    public int? IdGiangVien { get; set; }

    public string? YeuCauTruoc { get; set; }

    public string? HocDuoc { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? GiaBan { get; set; }

    [StringLength(500)]
    public string? HinhDaiDien { get; set; }

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

    [Column(TypeName = "datetime")]
    public DateTime? NgayPublish { get; set; }

    public int PhienBanHienTai { get; set; }

    public bool IsDeleted { get; set; }

    // ===== NAVIGATION PROPERTIES =====

    [ForeignKey("IdDanhMuc")]
    [InverseProperty("KhoaHocs")]
    public virtual DanhMucKhoaHoc? IdDanhMucNavigation { get; set; }

    [ForeignKey("IdGiangVien")]
    [InverseProperty("KhoaHocs")]
    public virtual NguoiDung? IdGiangVienNavigation { get; set; }

    // ✅ THÊM: Navigation property cho ChiTietChiaSeDoanhThu
    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<ChiTietChiaSeDoanhThu> ChiTietChiaSeDoanhThus { get; set; } = new List<ChiTietChiaSeDoanhThu>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<Chuong> Chuongs { get; set; } = new List<Chuong>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; } = new List<DangKyKhoaHoc>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<DanhGiaKhoaHoc> DanhGiaKhoaHocs { get; set; } = new List<DanhGiaKhoaHoc>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<KhoaHocKhuyenMai> KhoaHocKhuyenMais { get; set; } = new List<KhoaHocKhuyenMai>();

    // ✅ THÊM: Navigation property cho TienDoHocTap
    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<TienDoHocTap> TienDoHocTaps { get; set; } = new List<TienDoHocTap>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<KhoaHocPhienBan> KhoaHocPhienBans { get; set; } = new List<KhoaHocPhienBan>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("IdKhoaHocNavigation")]
    public virtual ICollection<KiemDuyetKhoaHoc> KiemDuyetKhoaHocs { get; set; } = new List<KiemDuyetKhoaHoc>();

}