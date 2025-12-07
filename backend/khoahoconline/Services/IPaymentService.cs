using khoahoconline.Dtos.Payment;

namespace khoahoconline.Services;

public interface IPaymentService
{
    Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto dto);
    Task<PaymentIntentDto> CreatePaymentIntentAsync(int orderId, int userId);
    Task<bool> ProcessPaymentSuccessAsync(string paymentIntentId);
    Task<OrderDto?> GetOrderByIdAsync(int orderId, int userId);
    Task<List<OrderDto>> GetUserOrdersAsync(int userId);
}














