namespace khoahoconline.Dtos.KhoaHoc
{
    /// <summary>
    /// DTO cho nội dung khóa học (curriculum)
    /// </summary>
    public class CurriculumDto
    {
        public int IdKhoaHoc { get; set; }
        public string? TenKhoaHoc { get; set; }
        
        // Thống kê
        public int TongSoChuong { get; set; }
        public int TongSoBaiGiang { get; set; }
        public int TongThoiLuong { get; set; } // Tính bằng phút
        
        // Danh sách chương
        public List<ChuongDto> Chuongs { get; set; } = new List<ChuongDto>();
    }
}