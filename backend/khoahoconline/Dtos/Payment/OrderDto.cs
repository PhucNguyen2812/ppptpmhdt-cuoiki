using khoahoconline.Dtos.KhoaHoc;

namespace khoahoconline.Dtos.Payment;

public class OrderDto
{
    public int Id { get; set; }
    public decimal TongTienGoc { get; set; }
    public decimal? TienGiam { get; set; }
    public decimal TongTienThanhToan { get; set; }
    public string TrangThaiDonHang { get; set; } = null!;
    public string? TrangThaiThanhToan { get; set; }
    public DateTime? NgayThanhToan { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public VoucherInfoDto? Voucher { get; set; }
}

public class OrderItemDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public decimal Price { get; set; }
    public string? CourseImage { get; set; }
    public string InstructorName { get; set; } = null!;
}

public class VoucherInfoDto
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
}














