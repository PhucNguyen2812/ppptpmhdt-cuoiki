using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using khoahoconline.Dtos;
using khoahoconline.Dtos.Voucher;
using khoahoconline.Services;

namespace khoahoconline.Controllers;

[Route("api/v1/vouchers")]
[ApiController]
public class VoucherController : ControllerBase
{
    private readonly ILogger<VoucherController> _logger;
    private readonly IVoucherService _voucherService;

    public VoucherController(
        ILogger<VoucherController> logger,
        IVoucherService voucherService)
    {
        _logger = logger;
        _voucherService = voucherService;
    }

    /// <summary>
    /// [PUBLIC] Validate voucher code
    /// </summary>
    [HttpPost("validate")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateVoucher([FromBody] ValidateVoucherDto dto)
    {
        _logger.LogInformation($"Validating voucher: {dto.Code}");
        
        var result = await _voucherService.ValidateVoucherAsync(dto.Code, dto.CourseIds);
        
        return Ok(ApiResponse<VoucherDto>.SuccessResponse(result, result.IsValid ? "Mã voucher hợp lệ" : result.Message ?? "Mã voucher không hợp lệ"));
    }
}












