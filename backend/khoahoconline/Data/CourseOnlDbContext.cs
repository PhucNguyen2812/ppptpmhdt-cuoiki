using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using khoahoconline.Data.Entities;

namespace khoahoconline.Data;

public partial class CourseOnlDbContext : DbContext
{
    public CourseOnlDbContext()
    {
    }

    public CourseOnlDbContext(DbContextOptions<CourseOnlDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BaiGiang> BaiGiangs { get; set; }

    public virtual DbSet<ChiTietChiaSeDoanhThu> ChiTietChiaSeDoanhThus { get; set; }

    public virtual DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }

    public virtual DbSet<Chuong> Chuongs { get; set; }

    public virtual DbSet<DangKyKhoaHoc> DangKyKhoaHocs { get; set; }

    public virtual DbSet<DanhGiaKhoaHoc> DanhGiaKhoaHocs { get; set; }

    public virtual DbSet<DanhMucKhoaHoc> DanhMucKhoaHocs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<KhoaHoc> KhoaHocs { get; set; }

    public virtual DbSet<KhoaHocKhuyenMai> KhoaHocKhuyenMais { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }

    public virtual DbSet<LichSuChuyenTien> LichSuChuyenTiens { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NguoiDungVaiTro> NguoiDungVaiTros { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<TaiLieuBaiGiang> TaiLieuBaiGiangs { get; set; }

    public virtual DbSet<TienDoHocTap> TienDoHocTaps { get; set; }

    public virtual DbSet<TienDoHocTapChiTiet> TienDoHocTapChiTiets { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-170JDGQ;Database=cuoikyoopQuanLyKhoaHocTrucTuyen1;User Id=sa;Password=thaithong123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaiGiang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BaiGiang__3214EC071FEAD29F");

            entity.Property(e => e.MienPhiXem).HasDefaultValue(false);
            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.IdChuongNavigation).WithMany(p => p.BaiGiangs).HasConstraintName("FK_BaiGiang_Chuong");
        });

        modelBuilder.Entity<ChiTietChiaSeDoanhThu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietC__3214EC07BC70BFCC");

            entity.HasOne(d => d.IdDonHangNavigation).WithMany(p => p.ChiTietChiaSeDoanhThus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietChiaSe_DonHang");

            entity.HasOne(d => d.IdGiangVienNavigation).WithMany(p => p.ChiTietChiaSeDoanhThus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietChiaSe_GiangVien");

            entity.HasOne(d => d.IdKhoaHocNavigation).WithMany(p => p.ChiTietChiaSeDoanhThus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietChiaSe_KhoaHoc");
        });

        modelBuilder.Entity<ChiTietGioHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietG__3214EC075971A2B8");

            entity.HasOne(d => d.IdGioHangNavigation).WithMany(p => p.ChiTietGioHangs).HasConstraintName("FK_ChiTietGioHang_GioHang");

            entity.HasOne(d => d.IdKhoaHocNavigation).WithMany(p => p.ChiTietGioHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietGioHang_KhoaHoc");
        });

        modelBuilder.Entity<Chuong>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Chuong__3214EC07C707E6BB");

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SoLuongBaiGiang).HasDefaultValue(0);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.IdKhoaHocNavigation).WithMany(p => p.Chuongs)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Chuong_KhoaHoc");
        });

        modelBuilder.Entity<DangKyKhoaHoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DangKyKh__3214EC07B1099F46");

            entity.Property(e => e.NgayDangKy).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.IdDonHangNavigation).WithMany(p => p.DangKyKhoaHocs).HasConstraintName("FK_DangKyKhoaHoc_DonHang");

            entity.HasOne(d => d.IdHocVienNavigation).WithMany(p => p.DangKyKhoaHocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DangKyKhoaHoc_HocVien");

            entity.HasOne(d => d.IdKhoaHocNavigation).WithMany(p => p.DangKyKhoaHocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DangKyKhoaHoc_KhoaHoc");
        });

        modelBuilder.Entity<DanhGiaKhoaHoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DanhGiaK__3214EC0761ED79A9");

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayDanhGia).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdHocVienNavigation).WithMany(p => p.DanhGiaKhoaHocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DanhGiaKhoaHoc_HocVien");

            entity.HasOne(d => d.IdKhoaHocNavigation).WithMany(p => p.DanhGiaKhoaHocs).HasConstraintName("FK_DanhGiaKhoaHoc_KhoaHoc");
        });

        modelBuilder.Entity<DanhMucKhoaHoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DanhMucK__3214EC0756243C77");

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DonHang__3214EC07531E57A9");

            entity.Property(e => e.TienGiam).HasDefaultValue(0m);

            entity.HasOne(d => d.IdNguoiDungNavigation).WithMany(p => p.DonHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonHang_NguoiDung");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GioHang__3214EC07A7F5DF5B");

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdNguoiDungNavigation).WithOne(p => p.GioHang).HasConstraintName("FK_GioHang_NguoiDung");
        });

        modelBuilder.Entity<KhoaHoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KhoaHoc__3214EC0701DFB60E");

            entity.Property(e => e.DiemDanhGia).HasDefaultValue(0.0m);
            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SoHocVien).HasDefaultValue(0);
            entity.Property(e => e.SoLuongDanhGia).HasDefaultValue(0);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.IdDanhMucNavigation).WithMany(p => p.KhoaHocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KhoaHoc_DanhMuc");

            entity.HasOne(d => d.IdGiangVienNavigation).WithMany(p => p.KhoaHocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KhoaHoc_GiangVien");
        });

        modelBuilder.Entity<KhoaHocKhuyenMai>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KhoaHocK__3214EC07E99C3FB7");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SoLuotDaSuDung).HasDefaultValue(0);

            entity.HasOne(d => d.IdKhoaHocNavigation).WithMany(p => p.KhoaHocKhuyenMais)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_KhoaHocKhuyenMai_KhoaHoc");

            entity.HasOne(d => d.IdKhuyenMaiNavigation).WithMany(p => p.KhoaHocKhuyenMais).HasConstraintName("FK_KhoaHocKhuyenMai_KhuyenMai");
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KhuyenMa__3214EC07E7D0A58D");

            entity.Property(e => e.GioiHanSoLuot).HasDefaultValue(false);
            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.IdNguoiTaoNavigation).WithMany(p => p.KhuyenMais).HasConstraintName("FK_KhuyenMai_NguoiTao");
        });

        modelBuilder.Entity<LichSuChuyenTien>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LichSuCh__3214EC0716043F96");

            entity.Property(e => e.NgayChuyenTien).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdChiTietChiaSeNavigation).WithMany(p => p.LichSuChuyenTiens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LichSuChuyenTien_ChiTietChiaSe");

            entity.HasOne(d => d.IdGiangVienNavigation).WithMany(p => p.LichSuChuyenTiens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LichSuChuyenTien_GiangVien");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NguoiDun__3214EC079DBBDB92");

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<NguoiDungVaiTro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NguoiDun__3214EC070948A5FB");

            entity.HasOne(d => d.IdNguoiDungNavigation).WithMany(p => p.NguoiDungVaiTros)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_NguoiDung_VaiTro_NguoiDung");

            entity.HasOne(d => d.IdVaiTroNavigation).WithMany(p => p.NguoiDungVaiTros)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_NguoiDung_VaiTro_VaiTro");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC076EAAAC0F");

            entity.Property(e => e.DaHuy).HasDefaultValue(false);
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdNguoiDungNavigation).WithMany(p => p.RefreshTokens)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_RefreshToken_NguoiDung");
        });

        modelBuilder.Entity<TaiLieuBaiGiang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TaiLieuB__3214EC07A722C144");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ThuTu).HasDefaultValue(0);

            entity.HasOne(d => d.IdBaiGiangNavigation).WithMany(p => p.TaiLieuBaiGiangs).HasConstraintName("FK_TaiLieuBaiGiang_BaiGiang");
        });

        modelBuilder.Entity<TienDoHocTap>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TienDoHo__3214EC0790FFA122");

            entity.Property(e => e.PhanTramHoanThanh).HasDefaultValue(0.0m);
            entity.Property(e => e.SoBaiHocDaHoanThanh).HasDefaultValue(0);

            entity.HasOne(d => d.IdDangKyKhoaHocNavigation).WithOne(p => p.TienDoHocTap).HasConstraintName("FK_TienDoHocTap_DangKyKhoaHoc");

            entity.HasOne(d => d.IdHocVienNavigation).WithMany(p => p.TienDoHocTaps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TienDoHocTap_HocVien");

            entity.HasOne(d => d.IdKhoaHocNavigation).WithMany(p => p.TienDoHocTaps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TienDoHocTap_KhoaHoc");
        });

        modelBuilder.Entity<TienDoHocTapChiTiet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TienDoHo__3214EC07F7FBC2C2");

            entity.Property(e => e.DaHoanThanh).HasDefaultValue(false);

            entity.HasOne(d => d.IdBaiGiangNavigation).WithMany(p => p.TienDoHocTapChiTiets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TienDoHocTapChiTiet_BaiGiang");

            entity.HasOne(d => d.IdTienDoHocTapNavigation).WithMany(p => p.TienDoHocTapChiTiets).HasConstraintName("FK_TienDoHocTapChiTiet_TienDoHocTap");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VaiTro__3214EC075675FD1C");

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
