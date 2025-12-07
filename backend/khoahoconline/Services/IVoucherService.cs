using khoahoconline.Dtos.Voucher;

namespace khoahoconline.Services;

public interface IVoucherService
{
    Task<VoucherDto> ValidateVoucherAsync(string code, List<int> courseIds);
    Task<VoucherDto?> GetVoucherByCodeAsync(string code);
}














