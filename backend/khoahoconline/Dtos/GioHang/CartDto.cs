namespace khoahoconline.Dtos.GioHang
{
    /// <summary>
    /// DTO cho giỏ hàng
    /// </summary>
    public class CartDto
    {
        public int Id { get; set; }
        public int IdNguoiDung { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal? TongTienGoc { get; set; }
        public decimal? PhanTramGiam { get; set; }
        public decimal? TienGiamVoucher { get; set; }
        public decimal? TongTienThanhToan { get; set; }
        public int? SoLuongKhoaHoc { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
    }
}







