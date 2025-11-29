using System.ComponentModel.DataAnnotations;

namespace khoahoconline.Dtos.DanhMuc
{
    public class UpdateDanhMucDto
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        public string TenDanhMuc { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? MoTa { get; set; }
    }
}