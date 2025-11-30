using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Entities;

[Table("KiemDuyetKhoaHoc")]
public partial class KiemDuyetKhoaHoc
{
    [Key]
    public int Id { get; set; }

    public int IdKhoaHoc { get; set; }

    public int PhienBan { get; set; }

    [StringLength(50)]
    public string TrangThaiKiemDuyet { get; set; } = null!;

    public int IdNguoiGui { get; set; }

    public int? IdNguoiKiemDuyet { get; set; }

    [StringLength(500)]
    public string? LyDoTuChoi { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayGui { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayKiemDuyet { get; set; }

    public string? NoiDungThayDoi { get; set; }

    [StringLength(500)]
    public string? GhiChu { get; set; }

    [ForeignKey("IdKhoaHoc")]
    [InverseProperty("KiemDuyetKhoaHocs")]
    public virtual KhoaHoc IdKhoaHocNavigation { get; set; } = null!;

    [ForeignKey("IdNguoiGui")]
    [InverseProperty("KiemDuyetKhoaHocGui")]
    public virtual NguoiDung IdNguoiGuiNavigation { get; set; } = null!;

    [ForeignKey("IdNguoiKiemDuyet")]
    [InverseProperty("KiemDuyetKhoaHocKiemDuyet")]
    public virtual NguoiDung? IdNguoiKiemDuyetNavigation { get; set; }
}







