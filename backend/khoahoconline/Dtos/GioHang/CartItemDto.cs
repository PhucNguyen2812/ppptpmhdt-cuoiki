namespace khoahoconline.Dtos.GioHang
{
    /// <summary>
    /// DTO cho một item trong giỏ hàng
    /// </summary>
    public class CartItemDto
    {
        public int Id { get; set; }
        public int IdKhoaHoc { get; set; }
        public string? TenKhoaHoc { get; set; }
        public string? MoTaNgan { get; set; }
        public string? HinhDaiDien { get; set; }
        public decimal? GiaBan { get; set; }
        public string? TenGiangVien { get; set; }
        public string? TenDanhMuc { get; set; }
        public decimal? DiemDanhGia { get; set; }
        public int? SoLuongDanhGia { get; set; }
    }
}







