using khoahoconline.Data;
using khoahoconline.Data.Entities;
using khoahoconline.Dtos.Voucher;
using khoahoconline.Middleware.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Services.Impl;

public class VoucherService : IVoucherService
{
    private readonly CourseOnlDbContext _context;
    private readonly ILogger<VoucherService> _logger;

    public VoucherService(CourseOnlDbContext context, ILogger<VoucherService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<VoucherDto?> GetVoucherByCodeAsync(string code)
    {
        var voucher = await _context.Vouchers
            .FirstOrDefaultAsync(v => v.MaCode == code);

        if (voucher == null) return null;

        return new VoucherDto
        {
            Id = voucher.Id,
            Code = voucher.MaCode ?? "",
            DiscountPercent = voucher.TiLeGiam ?? 0,
            IsValid = false, // Will be validated in ValidateVoucherAsync
            Message = null
        };
    }

    public async Task<VoucherDto> ValidateVoucherAsync(string code, List<int> courseIds)
    {
        _logger.LogInformation($"Validating voucher: {code} for {courseIds.Count} courses");

        var voucher = await _context.Vouchers
            .FirstOrDefaultAsync(v => v.MaCode == code);

        if (voucher == null)
        {
            return new VoucherDto
            {
                IsValid = false,
                Message = "Mã voucher không tồn tại"
            };
        }

        // Kiểm tra trạng thái
        if (voucher.TrangThai != true)
        {
            return new VoucherDto
            {
                IsValid = false,
                Message = "Mã voucher không còn hiệu lực"
            };
        }

        // Kiểm tra ngày hiệu lực
        var now = DateTime.UtcNow;
        if (voucher.NgayBatDau.HasValue && now < voucher.NgayBatDau.Value)
        {
            return new VoucherDto
            {
                IsValid = false,
                Message = "Mã voucher chưa có hiệu lực"
            };
        }

        if (voucher.NgayHetHan.HasValue && now > voucher.NgayHetHan.Value)
        {
            return new VoucherDto
            {
                IsValid = false,
                Message = "Mã voucher đã hết hạn"
            };
        }

        // Tính tổng tiền các khóa học
        var totalAmount = await _context.KhoaHocs
            .Where(k => courseIds.Contains(k.Id) && k.TrangThai == true)
            .SumAsync(k => k.GiaBan ?? 0);

        if (totalAmount == 0)
        {
            return new VoucherDto
            {
                IsValid = false,
                Message = "Không tìm thấy khóa học hợp lệ"
            };
        }

        // Tính số tiền giảm
        var discountPercent = voucher.TiLeGiam ?? 0;
        var discountAmount = totalAmount * (discountPercent / 100);

        return new VoucherDto
        {
            Id = voucher.Id,
            Code = voucher.MaCode ?? "",
            DiscountPercent = discountPercent,
            DiscountAmount = discountAmount,
            IsValid = true,
            Message = $"Giảm {discountPercent}% ({discountAmount:N0} VNĐ)"
        };
    }
}














