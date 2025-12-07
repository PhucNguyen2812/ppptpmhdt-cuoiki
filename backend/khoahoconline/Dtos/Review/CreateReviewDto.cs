using System.ComponentModel.DataAnnotations;

namespace khoahoconline.Dtos.Review
{
    public class CreateReviewDto
    {
        [Required(ErrorMessage = "ID khóa học là bắt buộc")]
        public int IdKhoaHoc { get; set; }

        [Required(ErrorMessage = "Điểm đánh giá là bắt buộc")]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5 sao")]
        public int DiemDanhGia { get; set; }

        [MaxLength(2000, ErrorMessage = "Bình luận không được vượt quá 2000 ký tự")]
        public string? BinhLuan { get; set; }
    }
}




