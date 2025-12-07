using AutoMapper;
using khoahoconline.Data.Repositories;
using khoahoconline.Dtos.Notification;
using khoahoconline.Middleware.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Services.Impl
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<NotificationService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedNotificationResult> GetNotificationsAsync(int userId, string? trangThai = null, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Getting notifications for user {userId}, trangThai: {trangThai}, page: {pageNumber}, size: {pageSize}");

            // Validate pagination
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            // Get all notifications for user
            var allNotifications = await _unitOfWork.NotificationRepository.GetByUserIdAsync(userId, trangThai);

            // Get unread count
            var unreadCount = await _unitOfWork.NotificationRepository.GetUnreadCountAsync(userId);

            // Apply pagination
            var totalCount = allNotifications.Count;
            var pagedNotifications = allNotifications
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Map to DTOs
            var notificationDtos = _mapper.Map<List<NotificationDto>>(pagedNotifications);

            return new PagedNotificationResult
            {
                Items = notificationDtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                UnreadCount = unreadCount
            };
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            _logger.LogInformation($"Getting unread count for user {userId}");
            return await _unitOfWork.NotificationRepository.GetUnreadCountAsync(userId);
        }

        public async Task MarkAsReadAsync(int notificationId, int userId)
        {
            _logger.LogInformation($"Marking notification {notificationId} as read for user {userId}");

            // Verify notification belongs to user
            var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(notificationId);
            if (notification == null)
            {
                throw new NotFoundException("Không tìm thấy thông báo");
            }

            if (notification.IdNguoiDung != userId)
            {
                throw new UnauthorizedException("Bạn không có quyền truy cập thông báo này");
            }

            await _unitOfWork.NotificationRepository.MarkAsReadAsync(notificationId);
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            _logger.LogInformation($"Marking all notifications as read for user {userId}");
            await _unitOfWork.NotificationRepository.MarkAllAsReadAsync(userId);
        }
    }
}




