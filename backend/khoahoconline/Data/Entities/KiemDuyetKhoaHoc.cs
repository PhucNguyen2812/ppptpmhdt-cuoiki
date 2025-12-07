using System;
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

    public int IdNguoiGui { get; set; } // ID giảng viên gửi yêu cầu

    [StringLength(50)]
    public string TrangThai { get; set; } = null!; // Chờ duyệt/Đã duyệt/Từ chối

    [StringLength(500)]
    public string? LyDoTuChoi { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayGui { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDuyet { get; set; }

    public int? IdNguoiDuyet { get; set; } // ID admin/kiểm duyệt viên duyệt

    [StringLength(1000)]
    public string? GhiChu { get; set; } // Ghi chú của người duyệt

    [ForeignKey("IdKhoaHoc")]
    [InverseProperty("KiemDuyetKhoaHocs")]
    public virtual KhoaHoc IdKhoaHocNavigation { get; set; } = null!;

    [ForeignKey("IdNguoiGui")]
    [InverseProperty("KiemDuyetKhoaHocGui")]
    public virtual NguoiDung IdNguoiGuiNavigation { get; set; } = null!;

    [ForeignKey("IdNguoiDuyet")]
    [InverseProperty("KiemDuyetKhoaHocDuyet")]
    public virtual NguoiDung? IdNguoiDuyetNavigation { get; set; }
}








