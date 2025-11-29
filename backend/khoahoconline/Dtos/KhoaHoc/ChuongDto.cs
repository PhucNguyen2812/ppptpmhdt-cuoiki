namespace khoahoconline.Dtos.KhoaHoc
{
    /// <summary>
    /// DTO cho chương học
    /// </summary>
    public class ChuongDto
    {
        public int Id { get; set; }
        public string? TenChuong { get; set; }
        public string? MoTa { get; set; }
        public int? ThuTu { get; set; }
        
        // Danh sách bài giảng trong chương
        public List<BaiGiangDto> BaiGiangs { get; set; } = new List<BaiGiangDto>();
        
        // Computed property - số lượng bài giảng
        public int SoLuongBaiGiang => BaiGiangs?.Count ?? 0;
    }
}