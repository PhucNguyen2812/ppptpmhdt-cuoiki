using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("KhoaHocPhienBan")]
[Index("IdKhoaHoc", "PhienBan", Name = "UQ_KhoaHoc_PhienBan", IsUnique = true)]
public partial class KhoaHocPhienBan
{
    [Key]
    public int Id { get; set; }

    public int IdKhoaHoc { get; set; }

    public int PhienBan { get; set; }

    [StringLength(200)]
    public string TenKhoaHoc { get; set; } = null!;

    [StringLength(500)]
    public string? MoTaNgan { get; set; }

    public string? MoTaChiTiet { get; set; }

    public int? IdDanhMuc { get; set; }

    public string? YeuCauTruoc { get; set; }

    public string? HocDuoc { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? GiaBan { get; set; }

    [StringLength(500)]
    public string? HinhDaiDien { get; set; }

    [StringLength(500)]
    public string? VideoGioiThieu { get; set; }

    [StringLength(200)]
    public string? VideoGioiThieuPublicId { get; set; }

    [StringLength(50)]
    public string? MucDo { get; set; }

    public string? NoiDungJSON { get; set; }

    [StringLength(50)]
    public string TrangThai { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime NgayTao { get; set; }

    [ForeignKey("IdKhoaHoc")]
    [InverseProperty("KhoaHocPhienBans")]
    public virtual KhoaHoc IdKhoaHocNavigation { get; set; } = null!;

    [ForeignKey("IdDanhMuc")]
    [InverseProperty("KhoaHocPhienBans")]
    public virtual DanhMucKhoaHoc? IdDanhMucNavigation { get; set; }
}
















