using System.ComponentModel.DataAnnotations;

namespace khoahoconline.Dtos.KhoaHoc
{
    public class UpdateCourseDto
    {
        [Required(ErrorMessage = "Tên khóa học không được để trống")]
        [StringLength(255, ErrorMessage = "Tên khóa học không được vượt quá 255 ký tự")]
        public string TenKhoaHoc { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Mô tả ngắn không được vượt quá 500 ký tự")]
        public string? MoTaNgan { get; set; }

        public string? MoTaChiTiet { get; set; }

        [Required(ErrorMessage = "Danh mục không được để trống")]
        public int IdDanhMuc { get; set; }

        [StringLength(50, ErrorMessage = "Mức độ không được vượt quá 50 ký tự")]
        public string? MucDo { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0")]
        public decimal? GiaBan { get; set; }

        [StringLength(500, ErrorMessage = "Hình đại diện không được vượt quá 500 ký tự")]
        public string? HinhDaiDien { get; set; }

        [StringLength(500, ErrorMessage = "Video giới thiệu không được vượt quá 500 ký tự")]
        public string? VideoGioiThieu { get; set; }

        public string? YeuCauTruoc { get; set; }

        public string? HocDuoc { get; set; }
    }
}





