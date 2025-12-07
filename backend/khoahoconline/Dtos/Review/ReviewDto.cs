namespace khoahoconline.Dtos.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int IdKhoaHoc { get; set; }
        public int IdHocVien { get; set; }
        public string HoTenHocVien { get; set; } = string.Empty;
        public string? AnhDaiDien { get; set; }
        public int DiemDanhGia { get; set; }
        public string? BinhLuan { get; set; }
        public DateTime? NgayDanhGia { get; set; }
        public DateTime? NgayCapNhat { get; set; }
    }
}




