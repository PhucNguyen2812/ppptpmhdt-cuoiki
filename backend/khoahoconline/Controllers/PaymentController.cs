using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using khoahoconline.Dtos;
using khoahoconline.Dtos.Payment;
using khoahoconline.Services;
using Stripe;

namespace khoahoconline.Controllers;

[Route("api/v1/payments")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly ILogger<PaymentController> _logger;
    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _configuration;

    public PaymentController(
        ILogger<PaymentController> logger,
        IPaymentService paymentService,
        IConfiguration configuration)
    {
        _logger = logger;
        _paymentService = paymentService;
        _configuration = configuration;
    }

    /// <summary>
    /// [AUTH] Create order from cart
    /// </summary>
    [HttpPost("orders")]
    [Authorize]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        _logger.LogInformation("Creating order");
        
        var userId = GetUserId();
        var result = await _paymentService.CreateOrderAsync(userId, dto);
        
        return Ok(ApiResponse<OrderDto>.SuccessResponse(result, "Tạo đơn hàng thành công"));
    }

    /// <summary>
    /// [AUTH] Create payment intent
    /// </summary>
    [HttpPost("create-intent")]
    [Authorize]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
    {
        _logger.LogInformation($"Creating payment intent for order {request.OrderId}");
        
        var userId = GetUserId();
        var result = await _paymentService.CreatePaymentIntentAsync(request.OrderId, userId);
        
        return Ok(ApiResponse<PaymentIntentDto>.SuccessResponse(result));
    }

    /// <summary>
    /// [PUBLIC] Stripe webhook endpoint
    /// </summary>
    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Webhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var webhookSecret = _configuration["Stripe:WebhookSecret"];

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                webhookSecret!
            );

            _logger.LogInformation($"Received webhook: {stripeEvent.Type}");

            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent != null)
                {
                    await _paymentService.ProcessPaymentSuccessAsync(paymentIntent.Id);
                    _logger.LogInformation($"Payment succeeded: {paymentIntent.Id}");
                }
            }
            else if (stripeEvent.Type == "payment_intent.payment_failed")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                _logger.LogWarning($"Payment failed: {paymentIntent?.Id}");
            }

            return Ok();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe webhook error");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook processing error");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// [AUTH] Get order by ID
    /// </summary>
    [HttpGet("orders/{id}")]
    [Authorize]
    public async Task<IActionResult> GetOrder(int id)
    {
        var userId = GetUserId();
        var result = await _paymentService.GetOrderByIdAsync(id, userId);
        
        if (result == null)
        {
            return NotFound(ApiResponse<OrderDto>.ErrorResponse("Không tìm thấy đơn hàng"));
        }
        
        return Ok(ApiResponse<OrderDto>.SuccessResponse(result));
    }

    /// <summary>
    /// [AUTH] Get user orders
    /// </summary>
    [HttpGet("orders")]
    [Authorize]
    public async Task<IActionResult> GetUserOrders()
    {
        var userId = GetUserId();
        var result = await _paymentService.GetUserOrdersAsync(userId);
        
        return Ok(ApiResponse<List<OrderDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// [AUTH] Confirm payment success (alternative to webhook for local development)
    /// </summary>
    [HttpPost("confirm-payment")]
    [Authorize]
    public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
    {
        _logger.LogInformation($"Confirming payment for payment intent: {request.PaymentIntentId}");
        
        var userId = GetUserId();
        var result = await _paymentService.ProcessPaymentSuccessAsync(request.PaymentIntentId);
        
        if (result)
        {
            return Ok(ApiResponse<object>.SuccessResponse(null, "Thanh toán đã được xử lý thành công"));
        }
        else
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Không thể xử lý thanh toán"));
        }
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Không thể xác định người dùng");
        }
        return userId;
    }
}

public class CreatePaymentIntentRequest
{
    public int OrderId { get; set; }
}

public class ConfirmPaymentRequest
{
    public string PaymentIntentId { get; set; } = string.Empty;
}


