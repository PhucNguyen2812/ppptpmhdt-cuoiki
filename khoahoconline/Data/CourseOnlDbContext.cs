using System;
using System.Collections.Generic;
using khoahoconline.Data.Entities;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }

    public virtual DbSet<ChiaSeLuanNhuan> ChiaSeLuanNhuans { get; set; }

    public virtual DbSet<DangKyKhoaHoc> DangKyKhoaHocs { get; set; }

    public virtual DbSet<DanhGiaKhoaHoc> DanhGiaKhoaHocs { get; set; }

    public virtual DbSet<DanhMucKhoaHoc> DanhMucKhoaHocs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<KhoaHoc> KhoaHocs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NguoiDungVaiTro> NguoiDungVaiTros { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<TaiLieuKhoaHoc> TaiLieuKhoaHocs { get; set; }

    public virtual DbSet<TienDoHocTap> TienDoHocTaps { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-170JDGQ;Database=cuoikyoopQuanLyKhoaHocTrucTuyen;User Id=sa;Password=thaithong123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaiGiang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BaiGiang__3214EC076780E4B7");

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
            entity.Property(e => e.XemThuMienPhi).HasDefaultValue(false);

            entity.HasOne(d => d.IdKhoaHocNavigation).WithMany(p => p.BaiGiangs).HasConstraintName("FK__BaiGiang__IdKhoa__60A75C0F");
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietD__3214EC070DB656AF");

            entity.HasOne(d => d.IdDonHangNavigation).WithMany(p => p.ChiTietDonHangs).HasConstraintName("FK__ChiTietDo__IdDon__7E37BEF6");

            entity.HasOne(d => d.IdKhoaHocNavigation).WithMany(p => p.ChiTietDonHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__IdKho__7F2BE32F");
        });

        modelBuilder.Entity<ChiTietGioHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietG__3214EC072C83834C");

            entity.HasOne(d => d.IdGioHangNavigation).WithMany(p => p.ChiTietGioHangs)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ChiTietGi__IdGio__74AE54BC");

            entity.HasOne(d => d.IdKhoaHocNavigation).WithMany(p => p.ChiTietGioHangs).HasConstraintName("FK__ChiTietGi__IdKho__75A278F5");
        });

        modelBuilder.Entity<ChiaSeLuanNhuan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiaSeLu__3214EC079F242507");

            entity.Property(e => e.TrangThaiThanhToan).HasDefaultValue(false);
            entity.Property(e => e.TyLeChiaGiangVien).HasDefaultValue(70m);

            entity.HasOne(d => d.IdDonHangNavigation).WithMany(p => p.ChiaSeLuanNhuans)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiaSeLua__IdDon__18EBB532");

            entity.HasOne(d => d.IdGiangVienNavigation).WithMany(p => p.ChiaSeLuanNhuans)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiaSeLua__IdGia__1AD3FDA4");

            entity.HasOne(d => d.IdKhoaHocNavigation).WithMany(p => p.ChiaSeLuanNhuans)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiaSeLua__IdKho__19DFD96B");
        });

        modelBuilder.Entity<DangKyKhoaHoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DangKyKh__3214EC07AD5D5C21");

            entity.Property(e => e.NgayDangKy).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdDonHangNavigation).WithMany(p => p.DangKyKhoaHocs).HasConstraintName("FK__DangKyKho__IdDon__05D8E0BE");

            entity.HasOne(d => d.IdHocVienNavigation).WithMany(p => p.DangKyKhoaHocs).HasConstraintName("FK__DangKyKho__IdHoc__03F0984C");

            entity.HasOne(d => d.IdKhoaHocNavigation).WithMany(p => p.DangKyKhoaHocs).HasConstraintName("FK__DangKyKho__IdKho__04E4BC85");
        });

        modelBuilder.Entity<DanhGiaKhoaHoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DanhGiaK__3214EC07CF98FE0F");

            entity.Property(e => e.NgayDanhGia).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.IdHocVienNavigation).WithMany(p => p.DanhGiaKhoaHocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGiaKh__IdHoc__1332DBDC");

            entity.HasOne(d => d.IdKhoaHocNavigation).WithMany(p => p.DanhGiaKhoaHocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGiaKh__IdKho__14270015");
        });

        modelBuilder.Entity<DanhMucKhoaHoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DanhMucK__3214EC07F28B142E");

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DonHang__3214EC07479D427C");

            entity.Property(e => e.NgayThanhToan).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TienGiam).HasDefaultValue(0m);

            entity.HasOne(d => d.IdHocVienNavigation).WithMany(p => p.DonHangs).HasConstraintName("FK__DonHang__IdHocVi__7A672E12");

            entity.HasOne(d => d.IdVoucherNavigation).WithMany(p => p.DonHangs).HasConstraintName("FK__DonHang__IdVouch__7B5B524B");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GioHang__3214EC0739D49BD8");

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PhanTramGiam).HasDefaultValue(0m);
            entity.Property(e => e.SoLuongKhoaHoc).HasDefaultValue(0);
            entity.Property(e => e.TienGiamVoucher).HasDefaultValue(0m);
            entity.Property(e => e.TongTienGoc).HasDefaultValue(0m);
            entity.Property(e => e.TongTienThanhToan).HasDefaultValue(0m);

            entity.HasOne(d => d.IdHocVienNavigation).WithOne(p => p.GioHang).HasConstraintName("FK__GioHang__IdHocVi__70DDC3D8");
        });

        modelBuilder.Entity<KhoaHoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KhoaHoc__3214EC072897117B");

            entity.Property(e => e.DiemDanhGia).HasDefaultValue(0m);
            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SoLuongDanhGia).HasDefaultValue(0);
            entity.Property(e => e.SoLuongHocVien).HasDefaultValue(0);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.IdDanhMucNavigation).WithMany(p => p.KhoaHocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhoaHoc__IdDanhM__5441852A");

            entity.HasOne(d => d.IdGiangVienNavigation).WithMany(p => p.KhoaHocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhoaHoc__IdGiang__5535A963");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NguoiDun__3214EC07D7AC2CE8");

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<NguoiDungVaiTro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NguoiDun__3214EC07816799B0");

            entity.HasOne(d => d.IdNguoiDungNavigation).WithMany(p => p.NguoiDungVaiTros).HasConstraintName("FK__NguoiDung__IdNgu__4316F928");

            entity.HasOne(d => d.IdVaiTroNavigation).WithMany(p => p.NguoiDungVaiTros).HasConstraintName("FK__NguoiDung__IdVai__440B1D61");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC07E8A799F7");

            entity.HasOne(d => d.IdNguoiDungNavigation).WithMany(p => p.RefreshTokens).HasConstraintName("FK__RefreshTo__IdNgu__46E78A0C");
        });

        modelBuilder.Entity<TaiLieuKhoaHoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TaiLieuK__3214EC07693FDB40");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ThuTu).HasDefaultValue(0);

            entity.HasOne(d => d.IdKhoaHocNavigation).WithMany(p => p.TaiLieuKhoaHocs)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__TaiLieuKh__IdKho__59FA5E80");
        });

        modelBuilder.Entity<TienDoHocTap>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TienDoHo__3214EC07B66DB074");

            entity.Property(e => e.DaHoanThanh).HasDefaultValue(false);
            entity.Property(e => e.PhanTramHoanThanh).HasDefaultValue(0m);

            entity.HasOne(d => d.IdDangKyNavigation).WithMany(p => p.TienDoHocTaps).HasConstraintName("FK__TienDoHoc__IdDan__0B91BA14");

            entity.HasOne(d => d.IdKhoaHocNavigation).WithMany(p => p.TienDoHocTaps).HasConstraintName("FK__TienDoHoc__IdKho__0C85DE4D");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VaiTro__3214EC07AA556544");

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Voucher__3214EC07377812C6");

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
