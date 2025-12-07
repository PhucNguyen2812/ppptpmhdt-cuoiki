using khoahoconline.Data.Entities;
using khoahoconline.Data.Repositories;
using khoahoconline.Dtos;
using khoahoconline.Dtos.KiemDuyetKhoaHoc;
using khoahoconline.Dtos.KhoaHoc;
using khoahoconline.Middleware.Exceptions;
using Chuong = khoahoconline.Data.Entities.Chuong;
using BaiGiang = khoahoconline.Data.Entities.BaiGiang;

namespace khoahoconline.Services.Impl;

public class KiemDuyetKhoaHocService : IKiemDuyetKhoaHocService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKhoaHocService _khoaHocService;
    private readonly ILogger<KiemDuyetKhoaHocService> _logger;

    public KiemDuyetKhoaHocService(
        IUnitOfWork unitOfWork,
        IKhoaHocService khoaHocService,
        ILogger<KiemDuyetKhoaHocService> logger)
    {
        _unitOfWork = unitOfWork;
        _khoaHocService = khoaHocService;
        _logger = logger;
    }

    public async Task<KiemDuyetKhoaHocDto> CreateApprovalRequestAsync(int khoaHocId, int instructorId)
    {
        _logger.LogInformation($"Creating approval request for course {khoaHocId} by instructor {instructorId}");

        // Verify course exists and belongs to instructor
        var course = await _unitOfWork.KhoaHocRepository.GetByIdAsync(khoaHocId);
        if (course == null)
        {
            throw new NotFoundException($"Không tìm thấy khóa học với id: {khoaHocId}");
        }

        if (course.IdGiangVien != instructorId)
        {
            throw new UnauthorizedException("Bạn không có quyền tạo yêu cầu kiểm duyệt cho khóa học này");
        }

        // Check if course already has a pending approval request
        var hasPending = await _unitOfWork.KiemDuyetKhoaHocRepository.HasPendingRequestAsync(khoaHocId);
        if (hasPending)
        {
            throw new BadRequestException("Khóa học này đã có yêu cầu kiểm duyệt đang chờ xử lý");
        }

        // Check if course is already published
        if (course.TrangThai == true && course.NgayPublish != null)
        {
            throw new BadRequestException("Khóa học này đã được publish rồi");
        }

        // Validate course has at least 1 chapter and each chapter has at least 1 lesson
        var chuongs = await _unitOfWork.KhoaHocRepository.GetChuongsByKhoaHocIdAsync(khoaHocId);
        if (chuongs == null || chuongs.Count == 0)
        {
            throw new BadRequestException("Khóa học phải có ít nhất 1 chương để gửi yêu cầu kiểm duyệt");
        }

        foreach (var chuong in chuongs)
        {
            var baiGiangs = await _unitOfWork.KhoaHocRepository.GetBaiGiangsByChuongIdAsync(chuong.Id);
            if (baiGiangs == null || baiGiangs.Count == 0)
            {
                throw new BadRequestException($"Chương '{chuong.TenChuong}' phải có ít nhất một bài giảng");
            }

            foreach (var baiGiang in baiGiangs)
            {
                if (string.IsNullOrWhiteSpace(baiGiang.DuongDanVideo))
                {
                    throw new BadRequestException($"Bài giảng '{baiGiang.TieuDe}' phải có video");
                }
            }
        }

        // Create approval request
        var request = new KiemDuyetKhoaHoc
        {
            IdKhoaHoc = khoaHocId,
            IdNguoiGui = instructorId,
            TrangThai = "Chờ duyệt",
            NgayGui = DateTime.UtcNow
        };

        await _unitOfWork.KiemDuyetKhoaHocRepository.CreateAsync(request);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Course approval request {request.Id} created for course {khoaHocId}");

        return await MapToDtoAsync(request);
    }

    public async Task<PagedResult<KiemDuyetKhoaHocDto>> GetApprovalRequestsAsync(string? trangThai = null, int pageNumber = 1, int pageSize = 10)
    {
        var pagedResult = await _unitOfWork.KiemDuyetKhoaHocRepository.GetPagedAsync(trangThai, pageNumber, pageSize);

        var items = new List<KiemDuyetKhoaHocDto>();
        foreach (var request in pagedResult.Items)
        {
            items.Add(await MapToDtoAsync(request));
        }

        return new PagedResult<KiemDuyetKhoaHocDto>
        {
            Items = items,
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize
        };
    }

    public async Task<KiemDuyetKhoaHocDetailDto> GetApprovalRequestByIdAsync(int id)
    {
        var request = await _unitOfWork.KiemDuyetKhoaHocRepository.GetByIdWithDetailsAsync(id);
        if (request == null)
        {
            throw new NotFoundException($"Không tìm thấy yêu cầu kiểm duyệt với id: {id}");
        }

        var dto = new KiemDuyetKhoaHocDetailDto
        {
            Id = request.Id,
            IdKhoaHoc = request.IdKhoaHoc,
            IdNguoiGui = request.IdNguoiGui,
            TenGiangVien = request.IdNguoiGuiNavigation?.HoTen,
            EmailGiangVien = request.IdNguoiGuiNavigation?.Email,
            TrangThai = request.TrangThai,
            LyDoTuChoi = request.LyDoTuChoi,
            NgayGui = request.NgayGui,
            NgayDuyet = request.NgayDuyet,
            IdNguoiDuyet = request.IdNguoiDuyet,
            TenNguoiDuyet = request.IdNguoiDuyetNavigation?.HoTen,
            GhiChu = request.GhiChu
        };

        // Map course details
        if (request.IdKhoaHocNavigation != null)
        {
            dto.KhoaHoc = await _khoaHocService.GetByIdAsync(request.IdKhoaHoc);
        }

        return dto;
    }

    public async Task<bool> ApproveCourseAsync(int requestId, int approverId, ApproveCourseApprovalDto? dto = null)
    {
        _logger.LogInformation($"Approving course request {requestId} by approver {approverId}");

        var request = await _unitOfWork.KiemDuyetKhoaHocRepository.GetByIdAsync(requestId);
        if (request == null)
        {
            throw new NotFoundException($"Không tìm thấy yêu cầu kiểm duyệt với id: {requestId}");
        }

        if (request.TrangThai != "Chờ duyệt")
        {
            throw new BadRequestException($"Yêu cầu không ở trạng thái chờ duyệt. Trạng thái hiện tại: {request.TrangThai}");
        }

        var course = await _unitOfWork.KhoaHocRepository.GetByIdAsync(request.IdKhoaHoc);
        if (course == null)
        {
            throw new NotFoundException($"Không tìm thấy khóa học với id: {request.IdKhoaHoc}");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Update request status
            request.TrangThai = "Đã duyệt";
            request.NgayDuyet = DateTime.UtcNow;
            request.IdNguoiDuyet = approverId;
            request.GhiChu = dto?.GhiChu;
            await _unitOfWork.KiemDuyetKhoaHocRepository.UpdateAsync(request);

            // Publish course
            var now = DateTime.UtcNow;
            course.TrangThai = true;
            course.NgayPublish = now;
            course.NgayCapNhat = now;
            await _unitOfWork.KhoaHocRepository.UpdateAsync(course);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            // Send notification to instructor
            try
            {
                await _unitOfWork.NotificationRepository.CreateNotificationAsync(
                    request.IdNguoiGui,
                    "Khóa học đã được duyệt",
                    $"Khóa học '{course.TenKhoaHoc}' của bạn đã được duyệt và đã được publish.",
                    "Khóa học được duyệt",
                    request.IdKhoaHoc
                );
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send notification to instructor");
            }

            _logger.LogInformation($"Course {request.IdKhoaHoc} approved successfully");
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> RejectCourseAsync(int requestId, int approverId, RejectCourseApprovalDto dto)
    {
        _logger.LogInformation($"Rejecting course request {requestId} by approver {approverId}");

        var request = await _unitOfWork.KiemDuyetKhoaHocRepository.GetByIdAsync(requestId);
        if (request == null)
        {
            throw new NotFoundException($"Không tìm thấy yêu cầu kiểm duyệt với id: {requestId}");
        }

        if (request.TrangThai != "Chờ duyệt")
        {
            throw new BadRequestException($"Yêu cầu không ở trạng thái chờ duyệt. Trạng thái hiện tại: {request.TrangThai}");
        }

        var course = await _unitOfWork.KhoaHocRepository.GetByIdAsync(request.IdKhoaHoc);
        if (course == null)
        {
            throw new NotFoundException($"Không tìm thấy khóa học với id: {request.IdKhoaHoc}");
        }

        // Update request status
        request.TrangThai = "Từ chối";
        request.NgayDuyet = DateTime.UtcNow;
        request.IdNguoiDuyet = approverId;
        request.LyDoTuChoi = dto.LyDoTuChoi;
        request.GhiChu = dto.GhiChu;

        await _unitOfWork.KiemDuyetKhoaHocRepository.UpdateAsync(request);
        await _unitOfWork.SaveChangesAsync();

        // Send notification to instructor
        try
        {
            await _unitOfWork.NotificationRepository.CreateNotificationAsync(
                request.IdNguoiGui,
                "Khóa học bị từ chối",
                $"Khóa học '{course.TenKhoaHoc}' của bạn đã bị từ chối. Lý do: {dto.LyDoTuChoi}",
                "Khóa học bị từ chối",
                request.IdKhoaHoc
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send notification to instructor");
        }

        _logger.LogInformation($"Course {request.IdKhoaHoc} rejected");
        return true;
    }

    private async Task<KiemDuyetKhoaHocDto> MapToDtoAsync(KiemDuyetKhoaHoc request)
    {
        // Load navigation properties if not loaded
        if (request.IdKhoaHocNavigation == null)
        {
            request = await _unitOfWork.KiemDuyetKhoaHocRepository.GetByIdWithDetailsAsync(request.Id) ?? request;
        }

        return new KiemDuyetKhoaHocDto
        {
            Id = request.Id,
            IdKhoaHoc = request.IdKhoaHoc,
            TenKhoaHoc = request.IdKhoaHocNavigation?.TenKhoaHoc,
            IdNguoiGui = request.IdNguoiGui,
            TenGiangVien = request.IdNguoiGuiNavigation?.HoTen,
            EmailGiangVien = request.IdNguoiGuiNavigation?.Email,
            TrangThai = request.TrangThai,
            LyDoTuChoi = request.LyDoTuChoi,
            NgayGui = request.NgayGui,
            NgayDuyet = request.NgayDuyet,
            IdNguoiDuyet = request.IdNguoiDuyet,
            TenNguoiDuyet = request.IdNguoiDuyetNavigation?.HoTen,
            GhiChu = request.GhiChu
        };
    }
}

