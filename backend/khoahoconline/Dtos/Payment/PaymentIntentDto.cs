namespace khoahoconline.Dtos.Payment;

public class PaymentIntentDto
{
    public string ClientSecret { get; set; } = null!;
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
}





