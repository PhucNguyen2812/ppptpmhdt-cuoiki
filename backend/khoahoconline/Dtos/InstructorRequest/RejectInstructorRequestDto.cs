using System.ComponentModel.DataAnnotations;

namespace khoahoconline.Dtos.InstructorRequest;

public class RejectInstructorRequestDto
{
    [Required(ErrorMessage = "Lý do từ chối là bắt buộc")]
    [StringLength(500, ErrorMessage = "Lý do từ chối không được vượt quá 500 ký tự")]
    public string LyDoTuChoi { get; set; } = null!;
}








