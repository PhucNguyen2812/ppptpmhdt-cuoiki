using System.ComponentModel.DataAnnotations;

namespace khoahoconline.Dtos.KiemDuyetKhoaHoc;

public class RejectCourseApprovalDto
{
    [Required(ErrorMessage = "Lý do từ chối là bắt buộc")]
    [StringLength(500, ErrorMessage = "Lý do từ chối không được vượt quá 500 ký tự")]
    public string LyDoTuChoi { get; set; } = null!;

    [StringLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự")]
    public string? GhiChu { get; set; }
}








