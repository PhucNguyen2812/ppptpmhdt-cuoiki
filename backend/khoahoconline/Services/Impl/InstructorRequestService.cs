using Microsoft.EntityFrameworkCore;
using khoahoconline.Data.Entities;
using khoahoconline.Data.Repositories;
using khoahoconline.Dtos;
using khoahoconline.Dtos.InstructorRequest;
using khoahoconline.Middleware.Exceptions;

namespace khoahoconline.Services.Impl;

public class InstructorRequestService : IInstructorRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InstructorRequestService> _logger;

    public InstructorRequestService(
        IUnitOfWork unitOfWork,
        ILogger<InstructorRequestService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<InstructorRequestDto> CreateRequestAsync(int userId, IFormFile chungChiFile, CreateInstructorRequestDto dto)
    {
        _logger.LogInformation($"Creating instructor request for user {userId}");

        // Validate user
        var user = await _unitOfWork.NguoiDungRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("Không tìm thấy người dùng");
        }

        if (user.TrangThai == false)
        {
            throw new BadRequestException("Tài khoản đã bị vô hiệu hóa");
        }

        // Kiểm tra xem đã có role GIANGVIEN chưa
        var hasInstructorRole = await _unitOfWork.NguoiDungRepository.HasRoleAsync(userId, "GIANGVIEN");
        if (hasInstructorRole)
        {
            throw new BadRequestException("Bạn đã là giảng viên rồi");
        }

        // Kiểm tra xem đã có yêu cầu chờ duyệt chưa
        var hasPendingRequest = await _unitOfWork.YeuCauDangKyGiangVienRepository.HasPendingRequestAsync(userId);
        if (hasPendingRequest)
        {
            throw new BadRequestException("Bạn đã có yêu cầu đang chờ duyệt");
        }

        // Validate file
        if (chungChiFile == null || chungChiFile.Length == 0)
        {
            throw new BadRequestException("Chứng chỉ là bắt buộc");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        var extension = Path.GetExtension(chungChiFile.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(extension))
        {
            throw new BadRequestException("Chứng chỉ phải là file ảnh (JPG, PNG) hoặc PDF");
        }

        if (chungChiFile.Length > 10 * 1024 * 1024) // 10MB
        {
            throw new BadRequestException("Kích thước file không được vượt quá 10MB");
        }

        // Validate thông tin bổ sung
        if (!string.IsNullOrWhiteSpace(dto.ThongTinBoSung) && dto.ThongTinBoSung.Length > 5000)
        {
            throw new BadRequestException("Thông tin bổ sung không được vượt quá 5000 ký tự");
        }

        // Save file
        var fileName = $"chungchi_{userId}_{Guid.NewGuid()}{extension}";
        var uploadPath = Path.Combine("wwwroot", "uploads", "chungchi");
        
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var filePath = Path.Combine(uploadPath, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await chungChiFile.CopyToAsync(stream);
        }

        var chungChiUrl = $"/uploads/chungchi/{fileName}";

        // Create request
        var request = new YeuCauDangKyGiangVien
        {
            IdHocVien = userId,
            ChungChiPath = chungChiUrl,
            ThongTinBoSung = dto.ThongTinBoSung,
            TrangThai = "Chờ duyệt",
            NgayGui = DateTime.UtcNow
        };

        var dbContext = _unitOfWork.GetDbContext();
        await dbContext.YeuCauDangKyGiangViens.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Instructor request {request.Id} created for user {userId}");

        return await MapToDtoAsync(request);
    }

    public async Task<PagedResult<InstructorRequestDto>> GetRequestsAsync(string? trangThai = null, int pageNumber = 1, int pageSize = 10)
    {
        var pagedResult = await _unitOfWork.YeuCauDangKyGiangVienRepository.GetPagedAsync(trangThai, pageNumber, pageSize);

        var items = new List<InstructorRequestDto>();
        foreach (var request in pagedResult.Items)
        {
            items.Add(await MapToDtoAsync(request));
        }

        return new PagedResult<InstructorRequestDto>
        {
            Items = items,
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize
        };
    }

    public async Task<InstructorRequestDto> GetRequestByIdAsync(int id)
    {
        var request = await _unitOfWork.YeuCauDangKyGiangVienRepository.GetByIdWithDetailsAsync(id);
        if (request == null)
        {
            throw new NotFoundException($"Không tìm thấy yêu cầu với id: {id}");
        }

        return await MapToDtoAsync(request);
    }

    public async Task<InstructorRequestDto?> GetMyRequestAsync(int userId)
    {
        var request = await _unitOfWork.YeuCauDangKyGiangVienRepository.GetByHocVienIdAsync(userId);
        if (request == null)
        {
            return null;
        }

        return await MapToDtoAsync(request);
    }

    public async Task<bool> ApproveRequestAsync(int requestId, int approverId)
    {
        _logger.LogInformation($"Approving instructor request {requestId} by approver {approverId}");

        // Validate approver
        var approver = await _unitOfWork.NguoiDungRepository.GetByIdAsync(approverId);
        if (approver == null)
        {
            throw new NotFoundException("Không tìm thấy thông tin người duyệt");
        }

        if (approver.TrangThai == false)
        {
            throw new BadRequestException("Tài khoản người duyệt đã bị vô hiệu hóa");
        }

        var request = await _unitOfWork.YeuCauDangKyGiangVienRepository.GetByIdAsync(requestId);
        if (request == null)
        {
            throw new NotFoundException($"Không tìm thấy yêu cầu với id: {requestId}");
        }

        if (request.TrangThai != "Chờ duyệt")
        {
            throw new BadRequestException($"Yêu cầu không ở trạng thái chờ duyệt. Trạng thái hiện tại: {request.TrangThai}");
        }

        // Kiểm tra xem học viên đã có role GIANGVIEN chưa (tránh duplicate)
        var hasInstructorRole = await _unitOfWork.NguoiDungRepository.HasRoleAsync(request.IdHocVien, "GIANGVIEN");
        if (hasInstructorRole)
        {
            throw new BadRequestException("Học viên này đã là giảng viên rồi");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Update request status
            request.TrangThai = "Đã duyệt";
            request.NgayDuyet = DateTime.UtcNow;
            request.IdNguoiDuyet = approverId;
            await _unitOfWork.YeuCauDangKyGiangVienRepository.UpdateAsync(request);

            // Add GIANGVIEN role to user
            var instructorRole = await _unitOfWork.VaiTroRepository.GetByTenVaiTroAsync("GIANGVIEN");
            if (instructorRole == null)
            {
                throw new NotFoundException("Không tìm thấy vai trò giảng viên trong hệ thống");
            }

            await _unitOfWork.NguoiDungRepository.AddRoleToUserAsync(request.IdHocVien, instructorRole.Id);
            await _unitOfWork.SaveChangesAsync();

            // Send notification to student
            try
            {
                await _unitOfWork.NotificationRepository.CreateNotificationAsync(
                    request.IdHocVien,
                    "Yêu cầu đăng ký làm giảng viên đã được duyệt",
                    "Yêu cầu đăng ký làm giảng viên của bạn đã được duyệt. Bạn có thể bắt đầu tạo khóa học.",
                    "Yêu cầu duyệt",
                    null
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification for approved request {requestId}");
                // Continue even if notification fails
            }

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation($"Instructor request {requestId} approved successfully");
            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, $"Error approving instructor request {requestId}");
            throw;
        }
    }

    public async Task<bool> RejectRequestAsync(int requestId, int approverId, RejectInstructorRequestDto dto)
    {
        _logger.LogInformation($"Rejecting instructor request {requestId} by approver {approverId}");

        // Validate approver
        var approver = await _unitOfWork.NguoiDungRepository.GetByIdAsync(approverId);
        if (approver == null)
        {
            throw new NotFoundException("Không tìm thấy thông tin người duyệt");
        }

        if (approver.TrangThai == false)
        {
            throw new BadRequestException("Tài khoản người duyệt đã bị vô hiệu hóa");
        }

        // Validate dto
        if (dto == null || string.IsNullOrWhiteSpace(dto.LyDoTuChoi))
        {
            throw new BadRequestException("Lý do từ chối là bắt buộc");
        }

        var request = await _unitOfWork.YeuCauDangKyGiangVienRepository.GetByIdAsync(requestId);
        if (request == null)
        {
            throw new NotFoundException($"Không tìm thấy yêu cầu với id: {requestId}");
        }

        if (request.TrangThai != "Chờ duyệt")
        {
            throw new BadRequestException($"Yêu cầu không ở trạng thái chờ duyệt. Trạng thái hiện tại: {request.TrangThai}");
        }

        // Update request status
        request.TrangThai = "Từ chối";
        request.LyDoTuChoi = dto.LyDoTuChoi;
        request.NgayDuyet = DateTime.UtcNow;
        request.IdNguoiDuyet = approverId;

        await _unitOfWork.YeuCauDangKyGiangVienRepository.UpdateAsync(request);
        await _unitOfWork.SaveChangesAsync();

        // Send notification to student
        try
        {
            await _unitOfWork.NotificationRepository.CreateNotificationAsync(
                request.IdHocVien,
                "Yêu cầu đăng ký làm giảng viên bị từ chối",
                $"Yêu cầu đăng ký làm giảng viên của bạn đã bị từ chối. Lý do: {dto.LyDoTuChoi}",
                "Yêu cầu duyệt",
                null
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending notification for rejected request {requestId}");
            // Continue even if notification fails
        }

        _logger.LogInformation($"Instructor request {requestId} rejected successfully");
        return true;
    }

    private async Task<InstructorRequestDto> MapToDtoAsync(YeuCauDangKyGiangVien request)
    {
        // Load navigation properties if not loaded
        if (request.IdHocVienNavigation == null)
        {
            var loadedRequest = await _unitOfWork.YeuCauDangKyGiangVienRepository.GetByIdWithDetailsAsync(request.Id);
            if (loadedRequest == null)
            {
                throw new NotFoundException("Không tìm thấy yêu cầu");
            }
            request = loadedRequest;
        }

        return new InstructorRequestDto
        {
            Id = request.Id,
            IdHocVien = request.IdHocVien,
            HoTen = request.IdHocVienNavigation?.HoTen,
            Email = request.IdHocVienNavigation?.Email,
            ChungChiPath = request.ChungChiPath,
            ThongTinBoSung = request.ThongTinBoSung,
            TrangThai = request.TrangThai,
            LyDoTuChoi = request.LyDoTuChoi,
            NgayGui = request.NgayGui,
            NgayDuyet = request.NgayDuyet,
            IdNguoiDuyet = request.IdNguoiDuyet,
            TenNguoiDuyet = request.IdNguoiDuyetNavigation?.HoTen
        };
    }
}

