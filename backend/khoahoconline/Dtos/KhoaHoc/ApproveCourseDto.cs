using System.ComponentModel.DataAnnotations;

namespace khoahoconline.Dtos.KhoaHoc
{
    public class ApproveCourseDto
    {
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? GhiChu { get; set; }
    }
}





