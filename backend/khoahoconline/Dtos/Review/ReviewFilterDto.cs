namespace khoahoconline.Dtos.Review
{
    public class ReviewFilterDto
    {
        public int? IdKhoaHoc { get; set; }
        public int? DiemDanhGia { get; set; } // Filter by rating (1-5)
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}




