using System.ComponentModel.DataAnnotations;

namespace khoahoconline.Dtos.Voucher;

public class ValidateVoucherDto
{
    [Required]
    public string Code { get; set; } = null!;

    [Required]
    public List<int> CourseIds { get; set; } = new();
}

public class VoucherDto
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public bool IsValid { get; set; }
    public string? Message { get; set; }
}





