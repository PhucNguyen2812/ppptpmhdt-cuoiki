using System.ComponentModel.DataAnnotations;

namespace khoahoconline.Dtos.KhoaHoc
{
    public class CreateCourseWithCurriculumDto
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

        public string? YeuCauTruoc { get; set; }

        public string? HocDuoc { get; set; }

        /// <summary>
        /// Nếu true: Publish khóa học ngay (cần validate đầy đủ)
        /// Nếu false: Lưu nháp (không cần validate đầy đủ)
        /// </summary>
        public bool Publish { get; set; } = false;

        [Required(ErrorMessage = "Khóa học phải có ít nhất một chương")]
        [MinLength(1, ErrorMessage = "Khóa học phải có ít nhất một chương")]
        public List<CreateChuongDto> Chuongs { get; set; } = new List<CreateChuongDto>();
    }

    public class CreateChuongDto
    {
        [Required(ErrorMessage = "Tên chương không được để trống")]
        [StringLength(255, ErrorMessage = "Tên chương không được vượt quá 255 ký tự")]
        public string TenChuong { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Chương phải có ít nhất một bài giảng")]
        [MinLength(1, ErrorMessage = "Chương phải có ít nhất một bài giảng")]
        public List<CreateBaiGiangDto> BaiGiangs { get; set; } = new List<CreateBaiGiangDto>();
    }

    public class CreateBaiGiangDto
    {
        [Required(ErrorMessage = "Tiêu đề bài giảng không được để trống")]
        [StringLength(200, ErrorMessage = "Tiêu đề bài giảng không được vượt quá 200 ký tự")]
        public string TieuDe { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Bài giảng phải có video")]
        [StringLength(500, ErrorMessage = "Đường dẫn video không được vượt quá 500 ký tự")]
        public string DuongDanVideo { get; set; } = null!;

        public int? ThoiLuong { get; set; }

        public bool? MienPhiXem { get; set; }
    }
}







