using khoahoconline.Data;
using khoahoconline.Data.Entities;
using khoahoconline.Data.Repositories;
using khoahoconline.Dtos.Payment;
using khoahoconline.Dtos.Voucher;
using khoahoconline.Middleware.Exceptions;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.TestHelpers;

namespace khoahoconline.Services.Impl;

public class PaymentService : IPaymentService
{
    private readonly CourseOnlDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVoucherService _voucherService;
    private readonly ILogger<PaymentService> _logger;
    private readonly IConfiguration _configuration;
    private readonly PaymentIntentService _stripePaymentIntentService;
    private readonly TransferService _stripeTransferService;

    public PaymentService(
        CourseOnlDbContext context,
        IUnitOfWork unitOfWork,
        IVoucherService voucherService,
        ILogger<PaymentService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _voucherService = voucherService;
        _logger = logger;
        _configuration = configuration;

        // Initialize Stripe
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        _stripePaymentIntentService = new PaymentIntentService();
        _stripeTransferService = new TransferService();
    }

    public async Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto dto)
    {
        _logger.LogInformation($"Creating order for user {userId} with {dto.CourseIds.Count} courses");

        var now = DateTime.UtcNow;

        // Validate courses - kiểm tra đầy đủ theo đặc tả
        var courses = await _context.KhoaHocs
            .Where(k => dto.CourseIds.Contains(k.Id) 
                && k.TrangThai == true 
                && !k.IsDeleted
                && k.NgayPublish != null) // Đã được publish
            .Include(k => k.IdGiangVienNavigation)
            .ToListAsync();

        if (courses.Count != dto.CourseIds.Count)
        {
            // Kiểm tra chi tiết để báo lỗi cụ thể
            var invalidCourseIds = dto.CourseIds.Except(courses.Select(c => c.Id)).ToList();
            var invalidCourses = await _context.KhoaHocs
                .Where(k => invalidCourseIds.Contains(k.Id))
                .ToListAsync();

            foreach (var invalidCourse in invalidCourses)
            {
                if (invalidCourse.IsDeleted)
                {
                    throw new BadRequestException($"Khóa học '{invalidCourse.TenKhoaHoc}' đã bị xóa");
                }
                if (invalidCourse.TrangThai != true)
                {
                    throw new BadRequestException($"Khóa học '{invalidCourse.TenKhoaHoc}' không còn hoạt động");
                }
                if (invalidCourse.NgayPublish == null)
                {
                    throw new BadRequestException($"Khóa học '{invalidCourse.TenKhoaHoc}' chưa được publish");
                }
            }

            throw new BadRequestException("Một số khóa học không hợp lệ hoặc không còn hoạt động");
        }

        // Kiểm tra học viên chưa đăng ký các khóa học này
        var nowCheck = DateTime.UtcNow;
        var existingEnrollments = await _context.DangKyKhoaHocs
            .Where(dk => dk.IdHocVien == userId 
                && dto.CourseIds.Contains(dk.IdKhoaHoc) 
                && dk.TrangThai == true
                && (dk.NgayHetHan == null || dk.NgayHetHan > nowCheck)) // Chưa hết thời hạn
            .Include(dk => dk.IdKhoaHocNavigation)
            .ToListAsync();

        if (existingEnrollments.Any())
        {
            var enrolledCourseNames = existingEnrollments
                .Select(e => e.IdKhoaHocNavigation?.TenKhoaHoc ?? "Khóa học")
                .ToList();
            throw new BadRequestException($"Bạn đã đăng ký các khóa học sau và chưa hết hạn: {string.Join(", ", enrolledCourseNames)}");
        }

        // Calculate total
        var tongTienGoc = courses.Sum(c => c.GiaBan ?? 0);

        // Validate and apply voucher
        VoucherDto? voucherDto = null;
        int? voucherId = null;
        decimal? tienGiam = null;

        if (!string.IsNullOrWhiteSpace(dto.VoucherCode))
        {
            voucherDto = await _voucherService.ValidateVoucherAsync(dto.VoucherCode, dto.CourseIds);
            if (!voucherDto.IsValid)
            {
                throw new BadRequestException(voucherDto.Message ?? "Mã voucher không hợp lệ");
            }
            voucherId = voucherDto.Id;
            tienGiam = voucherDto.DiscountAmount;
        }

        var tongTienThanhToan = tongTienGoc - (tienGiam ?? 0);

        // Create order
        var donHang = new DonHang
        {
            IdNguoiDung = userId,
            IdVoucher = voucherId,
            TongTienGoc = tongTienGoc,
            TongTien = tongTienGoc,
            TienGiam = tienGiam,
            TongTienThanhToan = tongTienThanhToan,
            TrangThaiDonHang = "Chờ thanh toán",
            TrangThaiThanhToan = "Chưa thanh toán",
            PhuongThucThanhToan = "Stripe"
        };

        await _context.DonHangs.AddAsync(donHang);
        await _context.SaveChangesAsync();

        // Create order items
        foreach (var course in courses)
        {
            await _context.ChiTietDonHangs.AddAsync(new ChiTietDonHang
            {
                IdDonHang = donHang.Id,
                IdKhoaHoc = course.Id
            });
        }

        await _context.SaveChangesAsync();

        // Map to DTO
        return await MapToOrderDtoAsync(donHang);
    }

    public async Task<PaymentIntentDto> CreatePaymentIntentAsync(int orderId, int userId)
    {
        _logger.LogInformation($"Creating payment intent for order {orderId}");

        var donHang = await _context.DonHangs
            .Include(d => d.IdNguoiDungNavigation)
            .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.IdKhoaHocNavigation)
            .FirstOrDefaultAsync(d => d.Id == orderId && d.IdNguoiDung == userId);

        if (donHang == null)
        {
            throw new NotFoundException("Không tìm thấy đơn hàng");
        }

        if (donHang.TrangThaiDonHang != "Chờ thanh toán")
        {
            throw new BadRequestException("Đơn hàng không ở trạng thái chờ thanh toán");
        }

        // Validate lại các khóa học trong đơn hàng (có thể đã thay đổi sau khi tạo đơn)
        var now = DateTime.UtcNow;
        var invalidCourses = donHang.ChiTietDonHangs
            .Where(c => c.IdKhoaHocNavigation == null 
                || c.IdKhoaHocNavigation.IsDeleted 
                || c.IdKhoaHocNavigation.TrangThai != true
                || c.IdKhoaHocNavigation.NgayPublish == null)
            .Select(c => c.IdKhoaHocNavigation?.TenKhoaHoc ?? "Khóa học")
            .ToList();

        if (invalidCourses.Any())
        {
            throw new BadRequestException($"Một số khóa học trong đơn hàng không còn hợp lệ: {string.Join(", ", invalidCourses)}");
        }

        // Kiểm tra lại xem học viên đã đăng ký khóa học nào chưa (và chưa hết hạn)
        var courseIds = donHang.ChiTietDonHangs.Select(c => c.IdKhoaHoc).ToList();
        var nowCheckEnrollments = DateTime.UtcNow;
        var existingEnrollments = await _context.DangKyKhoaHocs
            .Where(dk => dk.IdHocVien == userId 
                && courseIds.Contains(dk.IdKhoaHoc) 
                && dk.TrangThai == true
                && (dk.NgayHetHan == null || dk.NgayHetHan > nowCheckEnrollments)) // Chưa hết thời hạn
            .Include(dk => dk.IdKhoaHocNavigation)
            .ToListAsync();

        if (existingEnrollments.Any())
        {
            var enrolledCourseNames = existingEnrollments
                .Select(e => e.IdKhoaHocNavigation?.TenKhoaHoc ?? "Khóa học")
                .ToList();
            throw new BadRequestException($"Bạn đã đăng ký các khóa học sau và chưa hết hạn: {string.Join(", ", enrolledCourseNames)}");
        }

        // Convert VND to cents (Stripe uses smallest currency unit)
        // 1 VND = 1 cent (Stripe doesn't support VND, but we'll use cents as VND)
        var amountInCents = (long)(donHang.TongTienThanhToan);

        var options = new PaymentIntentCreateOptions
        {
            Amount = amountInCents,
            Currency = "vnd", // Note: Stripe doesn't officially support VND, but we can use it for test
            // For production, you might want to convert to USD or use a different approach
            PaymentMethodTypes = new List<string> { "card" },
            Metadata = new Dictionary<string, string>
            {
                { "orderId", orderId.ToString() },
                { "userId", userId.ToString() }
            }
        };

        var paymentIntent = await _stripePaymentIntentService.CreateAsync(options);

        // Save payment intent ID to order
        donHang.StripePaymentIntentId = paymentIntent.Id;
        await _context.SaveChangesAsync();

        return new PaymentIntentDto
        {
            ClientSecret = paymentIntent.ClientSecret,
            OrderId = orderId,
            Amount = donHang.TongTienThanhToan
        };
    }

    public async Task<bool> ProcessPaymentSuccessAsync(string paymentIntentId)
    {
        _logger.LogInformation($"Processing payment success for payment intent: {paymentIntentId}");

        var donHang = await _context.DonHangs
            .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.IdKhoaHocNavigation)
            .FirstOrDefaultAsync(d => d.StripePaymentIntentId == paymentIntentId);

        if (donHang == null)
        {
            _logger.LogWarning($"Order not found for payment intent: {paymentIntentId}");
            return false;
        }

        if (donHang.TrangThaiThanhToan == "Đã thanh toán")
        {
            _logger.LogWarning($"Order {donHang.Id} already processed");
            return true; // Already processed
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Update order status theo đặc tả: "Hoàn thành" và "Đã thanh toán"
            donHang.TrangThaiDonHang = "Hoàn thành";
            donHang.TrangThaiThanhToan = "Đã thanh toán";
            donHang.NgayThanhToan = DateTime.UtcNow;

            // Get courses with instructors
            var courses = donHang.ChiTietDonHangs
                .Select(c => c.IdKhoaHocNavigation)
                .Where(c => c != null)
                .Cast<KhoaHoc>()
                .ToList();

            // Group courses by instructor for transfer
            var instructorGroups = courses
                .GroupBy(c => c.IdGiangVien)
                .ToList();

            // Process each course
            foreach (var course in courses)
            {
                // Calculate course price after voucher (proportional)
                var coursePrice = course.GiaBan ?? 0;
                var voucherDiscount = donHang.TienGiam ?? 0;
                var totalOriginalPrice = donHang.TongTienGoc;
                var courseDiscount = totalOriginalPrice > 0 
                    ? voucherDiscount * (coursePrice / totalOriginalPrice) 
                    : 0;
                var coursePriceAfterDiscount = coursePrice - courseDiscount;

                // Kiểm tra lại xem học viên đã đăng ký chưa và chưa hết hạn (tránh duplicate)
                var nowCheck = DateTime.UtcNow;
                var existingEnrollment = await _context.DangKyKhoaHocs
                    .FirstOrDefaultAsync(dk => dk.IdHocVien == donHang.IdNguoiDung 
                        && dk.IdKhoaHoc == course.Id 
                        && dk.TrangThai == true
                        && (dk.NgayHetHan == null || dk.NgayHetHan > nowCheck)); // Chưa hết thời hạn

                if (existingEnrollment != null)
                {
                    _logger.LogWarning($"User {donHang.IdNguoiDung} already enrolled in course {course.Id} and not expired, skipping enrollment creation");
                    continue; // Bỏ qua khóa học này
                }

                // Nếu đã đăng ký nhưng đã hết hạn, cập nhật lại thời hạn
                var expiredEnrollment = await _context.DangKyKhoaHocs
                    .FirstOrDefaultAsync(dk => dk.IdHocVien == donHang.IdNguoiDung 
                        && dk.IdKhoaHoc == course.Id 
                        && dk.TrangThai == true
                        && dk.NgayHetHan.HasValue 
                        && dk.NgayHetHan <= nowCheck); // Đã hết hạn

                if (expiredEnrollment != null)
                {
                    // Gia hạn: cập nhật lại thời hạn
                    const int thoiHanTruyCapGiaHan = 6; // Thời hạn truy cập: 6 tháng
                    expiredEnrollment.NgayDangKy = DateTime.UtcNow;
                    expiredEnrollment.NgayHetHan = DateTime.UtcNow.AddMonths(thoiHanTruyCapGiaHan);
                    expiredEnrollment.TrangThai = true;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Renewed enrollment for user {donHang.IdNguoiDung} in course {course.Id}");
                    continue; // Đã gia hạn, không cần tạo mới
                }

                // Create enrollment
                var ngayDangKy = DateTime.UtcNow;
                const int thoiHanTruyCap = 6; // Thời hạn truy cập mặc định: 6 tháng (có thể config trong appsettings)
                var dangKy = new DangKyKhoaHoc
                {
                    IdHocVien = donHang.IdNguoiDung,
                    IdKhoaHoc = course.Id,
                    IdDonHang = donHang.Id,
                    NgayDangKy = ngayDangKy,
                    NgayHetHan = ngayDangKy.AddMonths(thoiHanTruyCap), // Thời hạn truy cập: 6 tháng từ ngày đăng ký
                    TrangThai = true
                };
                await _context.DangKyKhoaHocs.AddAsync(dangKy);
                await _context.SaveChangesAsync();

                // Cập nhật SoLuongHocVien của khóa học
                course.SoLuongHocVien = (course.SoLuongHocVien ?? 0) + 1;
                _logger.LogInformation($"Updated SoLuongHocVien for course {course.Id} to {course.SoLuongHocVien}");

                // Create learning progress
                var tongSoBaiGiang = await _context.BaiGiangs
                    .Where(b => b.IdChuongNavigation!.IdKhoaHoc == course.Id && b.TrangThai == true)
                    .CountAsync();

                var tienDo = new TienDoHocTap
                {
                    IdDangKyKhoaHoc = dangKy.Id,
                    IdKhoaHoc = course.Id,
                    IdHocVien = donHang.IdNguoiDung,
                    SoBaiHocDaHoanThanh = 0,
                    TongSoBaiHoc = tongSoBaiGiang,
                    PhanTramHoanThanh = 0,
                    DaHoanThanh = false,
                    NgayBatDau = DateTime.UtcNow
                };
                await _context.TienDoHocTaps.AddAsync(tienDo);
                await _context.SaveChangesAsync();

                // Calculate revenue share (70% instructor, 30% admin)
                var tyLeChiaGiangVien = 70m;
                var tienGiangVien = (int)(coursePriceAfterDiscount * tyLeChiaGiangVien / 100);
                var tienHeThong = (int)(coursePriceAfterDiscount * 30 / 100);

                // Create revenue share record
                var chiTietChiaSe = new ChiTietChiaSeDoanhThu
                {
                    IdDonHang = donHang.Id,
                    IdKhoaHoc = course.Id,
                    IdGiangVien = course.IdGiangVien ?? 0,
                    GiaKhoaHoc = (int)coursePriceAfterDiscount,
                    TyLeChiaGiangVien = tyLeChiaGiangVien,
                    TienGiangVien = tienGiangVien,
                    TienHeThong = tienHeThong
                };
                await _context.ChiTietChiaSeDoanhThus.AddAsync(chiTietChiaSe);
            }

            await _context.SaveChangesAsync();

            // Process Stripe transfers (group by instructor)
            foreach (var instructorGroup in instructorGroups)
            {
                var instructorId = instructorGroup.Key;
                if (!instructorId.HasValue) continue;

                var instructor = await _context.NguoiDungs.FindAsync(instructorId.Value);
                if (instructor == null) continue;

                // Calculate total amount for this instructor
                var instructorCourses = instructorGroup.ToList();
                var totalInstructorAmount = instructorCourses
                    .Sum(c => 
                    {
                        var coursePrice = c!.GiaBan ?? 0;
                        var voucherDiscount = donHang.TienGiam ?? 0;
                        var totalOriginalPrice = donHang.TongTienGoc;
                        var courseDiscount = totalOriginalPrice > 0 
                            ? voucherDiscount * (coursePrice / totalOriginalPrice) 
                            : 0;
                        var coursePriceAfterDiscount = coursePrice - courseDiscount;
                        return (long)(coursePriceAfterDiscount * 0.7m); // 70%
                    });

                // Create transfer (in test mode, this is simulated)
                try
                {
                    // In test mode, we can create a transfer to a test account
                    // For now, we'll just log it and save the transfer ID as null
                    // In production, you would create actual transfers
                    var transferId = $"tr_test_{Guid.NewGuid()}"; // Simulated transfer ID

                    // Update ChiTietChiaSeDoanhThu with transfer ID
                    foreach (var course in instructorCourses)
                    {
                        var chiTiet = await _context.ChiTietChiaSeDoanhThus
                            .FirstOrDefaultAsync(c => 
                                c.IdDonHang == donHang.Id && 
                                c.IdKhoaHoc == course!.Id);
                        
                        if (chiTiet != null)
                        {
                            chiTiet.StripeTransferId = transferId;
                        }
                    }

                    _logger.LogInformation($"Transfer {totalInstructorAmount} to instructor {instructorId} (simulated in test mode)");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error creating transfer for instructor {instructorId}");
                    // Continue processing even if transfer fails
                }
            }

            // Xóa các khóa học đã thanh toán khỏi giỏ hàng
            var cart = await _unitOfWork.GioHangRepository.GetByUserIdAsync(donHang.IdNguoiDung);
            if (cart != null)
            {
                var courseIdsInOrder = donHang.ChiTietDonHangs.Select(c => c.IdKhoaHoc).ToList();
                var cartItemsToRemove = await _context.ChiTietGioHangs
                    .Where(c => c.IdGioHang == cart.Id && courseIdsInOrder.Contains(c.IdKhoaHoc))
                    .ToListAsync();

                if (cartItemsToRemove.Any())
                {
                    _context.ChiTietGioHangs.RemoveRange(cartItemsToRemove);
                    _logger.LogInformation($"Removed {cartItemsToRemove.Count} courses from cart after successful payment");
                    
                    // Cập nhật lại tổng tiền giỏ hàng
                    var remainingCartItems = await _context.ChiTietGioHangs
                        .Where(c => c.IdGioHang == cart.Id)
                        .Include(c => c.IdKhoaHocNavigation)
                        .ToListAsync();
                    
                    if (remainingCartItems.Any())
                    {
                        cart.TongTienGoc = remainingCartItems.Sum(c => c.IdKhoaHocNavigation?.GiaBan ?? 0);
                        cart.TongTienThanhToan = cart.TongTienGoc - (cart.TienGiamVoucher ?? 0);
                    }
                    else
                    {
                        // Nếu giỏ hàng trống, xóa luôn giỏ hàng
                        _context.GioHangs.Remove(cart);
                    }
                }
            }

            await _context.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation($"Payment processed successfully for order {donHang.Id}");
            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, $"Error processing payment for order {donHang.Id}");
            throw;
        }
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int orderId, int userId)
    {
        var donHang = await _context.DonHangs
            .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.IdKhoaHocNavigation)
                    .ThenInclude(k => k!.IdGiangVienNavigation)
            .Include(d => d.IdVoucherNavigation)
            .FirstOrDefaultAsync(d => d.Id == orderId && d.IdNguoiDung == userId);

        if (donHang == null) return null;

        return await MapToOrderDtoAsync(donHang);
    }

    public async Task<List<OrderDto>> GetUserOrdersAsync(int userId)
    {
        var donHangs = await _context.DonHangs
            .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.IdKhoaHocNavigation)
                    .ThenInclude(k => k!.IdGiangVienNavigation)
            .Include(d => d.IdVoucherNavigation)
            .Where(d => d.IdNguoiDung == userId)
            .OrderByDescending(d => d.Id)
            .ToListAsync();

        var orders = new List<OrderDto>();
        foreach (var donHang in donHangs)
        {
            orders.Add(await MapToOrderDtoAsync(donHang));
        }

        return orders;
    }

    private async Task<OrderDto> MapToOrderDtoAsync(DonHang donHang)
    {
        var items = new List<OrderItemDto>();

        foreach (var chiTiet in donHang.ChiTietDonHangs)
        {
            var course = chiTiet.IdKhoaHocNavigation;
            if (course == null) continue;

            items.Add(new OrderItemDto
            {
                CourseId = course.Id,
                CourseName = course.TenKhoaHoc ?? "",
                Price = course.GiaBan ?? 0,
                CourseImage = course.HinhDaiDien,
                InstructorName = course.IdGiangVienNavigation?.HoTen ?? "Giảng viên"
            });
        }

        VoucherInfoDto? voucherInfo = null;
        if (donHang.IdVoucherNavigation != null)
        {
            voucherInfo = new VoucherInfoDto
            {
                Id = donHang.IdVoucherNavigation.Id,
                Code = donHang.IdVoucherNavigation.MaCode ?? "",
                DiscountPercent = donHang.IdVoucherNavigation.TiLeGiam ?? 0,
                DiscountAmount = donHang.TienGiam ?? 0
            };
        }

        return new OrderDto
        {
            Id = donHang.Id,
            TongTienGoc = donHang.TongTienGoc ?? 0,
            TienGiam = donHang.TienGiam,
            TongTienThanhToan = donHang.TongTienThanhToan,
            TrangThaiDonHang = donHang.TrangThaiDonHang,
            TrangThaiThanhToan = donHang.TrangThaiThanhToan,
            NgayThanhToan = donHang.NgayThanhToan,
            Items = items,
            Voucher = voucherInfo
        };
    }
}







