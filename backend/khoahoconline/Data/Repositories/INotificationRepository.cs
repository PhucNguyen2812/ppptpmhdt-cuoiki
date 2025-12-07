using khoahoconline.Data.Entities;
using khoahoconline.Dtos;

namespace khoahoconline.Data.Repositories
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        Task<List<Notification>> GetByUserIdAsync(int userId, string? trangThai = null);
        Task<int> GetUnreadCountAsync(int userId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(int userId);
        Task CreateNotificationAsync(int userId, string tieuDe, string noiDung, string loai, int? idKhoaHoc = null);
        Task CreateNotificationsForCourseStudentsAsync(int courseId, string tieuDe, string noiDung, string loai);
    }
}








