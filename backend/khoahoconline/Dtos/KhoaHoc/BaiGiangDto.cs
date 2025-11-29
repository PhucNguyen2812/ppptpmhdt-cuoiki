namespace khoahoconline.Dtos.KhoaHoc
{
    /// <summary>
    /// DTO cho bài giảng
    /// </summary>
    public class BaiGiangDto
    {
        public int Id { get; set; }
        public string? TieuDe { get; set; }
        public string? MoTa { get; set; }
        public int? ThoiLuong { get; set; } // Tính bằng giây
        public int? ThuTu { get; set; }
        public bool? XemThuMienPhi { get; set; }
        
        // VideoUrl chỉ trả về nếu user đã mua khóa học hoặc XemThuMienPhi = true
        public string? VideoUrl { get; set; }
    }
}