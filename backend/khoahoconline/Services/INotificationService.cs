using khoahoconline.Dtos.Notification;

namespace khoahoconline.Services
{
    public interface INotificationService
    {
        Task<PagedNotificationResult> GetNotificationsAsync(int userId, string? trangThai = null, int pageNumber = 1, int pageSize = 10);
        Task<int> GetUnreadCountAsync(int userId);
        Task MarkAsReadAsync(int notificationId, int userId);
        Task MarkAllAsReadAsync(int userId);
    }
}




