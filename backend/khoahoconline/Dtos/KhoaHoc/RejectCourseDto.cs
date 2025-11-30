using System.ComponentModel.DataAnnotations;

namespace khoahoconline.Dtos.KhoaHoc
{
    public class RejectCourseDto
    {
        [Required(ErrorMessage = "Lý do từ chối không được để trống")]
        [StringLength(500, ErrorMessage = "Lý do từ chối không được vượt quá 500 ký tự")]
        public string LyDoTuChoi { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? GhiChu { get; set; }
    }
}





