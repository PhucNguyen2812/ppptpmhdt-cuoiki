using System.ComponentModel.DataAnnotations;

namespace khoahoconline.Dtos.Payment;

public class CreateOrderDto
{
    [Required]
    public List<int> CourseIds { get; set; } = new();

    public string? VoucherCode { get; set; }
}














