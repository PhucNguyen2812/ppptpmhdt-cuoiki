using System.ComponentModel.DataAnnotations;

namespace khoahoconline.Dtos.KiemDuyetKhoaHoc;

public class ApproveCourseApprovalDto
{
    [StringLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự")]
    public string? GhiChu { get; set; }
}





