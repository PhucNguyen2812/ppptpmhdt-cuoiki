namespace khoahoconline.Dtos.KhoaHoc
{
    /// <summary>
    /// DTO cho filter và search parameters
    /// </summary>
    public class KhoaHocFilterDto
    {
        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        // Search
        public string? Search { get; set; }

        // Filter
        public int? CategoryId { get; set; }
        public decimal? MinRating { get; set; } // Lọc DiemDanhGia >= MinRating
        public string? Level { get; set; } // Lọc theo MucDo

        // Sort
        public string? SortBy { get; set; } // best-selling, highest-rated, newest, price-low-high, price-high-low
    }
}