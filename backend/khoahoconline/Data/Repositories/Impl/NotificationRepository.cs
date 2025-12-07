using Microsoft.EntityFrameworkCore;
using khoahoconline.Data.Entities;
using khoahoconline.Dtos;

namespace khoahoconline.Data.Repositories.Impl
{
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(CourseOnlDbContext context) : base(context)
        {
        }

        public async Task<List<Notification>> GetByUserIdAsync(int userId, string? trangThai = null)
        {
            var query = _dbSet.AsQueryable()
                .Where(n => n.IdNguoiDung == userId);

            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(n => n.TrangThai == trangThai);
            }

            return await query
                .OrderByDescending(n => n.NgayTao)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _dbSet
                .CountAsync(n => n.IdNguoiDung == userId && n.TrangThai == "Chưa đọc");
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _dbSet.FindAsync(notificationId);
            if (notification != null)
            {
                notification.TrangThai = "Đã đọc";
                notification.NgayDoc = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            var notifications = await _dbSet
                .Where(n => n.IdNguoiDung == userId && n.TrangThai == "Chưa đọc")
                .ToListAsync();

            var now = DateTime.UtcNow;
            foreach (var notification in notifications)
            {
                notification.TrangThai = "Đã đọc";
                notification.NgayDoc = now;
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateNotificationAsync(int userId, string tieuDe, string noiDung, string loai, int? idKhoaHoc = null)
        {
            var notification = new Notification
            {
                IdNguoiDung = userId,
                TieuDe = tieuDe,
                NoiDung = noiDung,
                Loai = loai,
                TrangThai = "Chưa đọc",
                IdKhoaHoc = idKhoaHoc,
                NgayTao = DateTime.UtcNow
            };

            await CreateAsync(notification);
        }

        public async Task CreateNotificationsForCourseStudentsAsync(int courseId, string tieuDe, string noiDung, string loai)
        {
            // Lấy danh sách học viên đã đăng ký khóa học này
            var enrolledStudents = await _context.DangKyKhoaHocs
                .Where(dk => dk.IdKhoaHoc == courseId && dk.TrangThai == true)
                .Select(dk => dk.IdHocVien)
                .Distinct()
                .ToListAsync();

            if (enrolledStudents.Count == 0)
            {
                return; // Không có học viên nào đăng ký
            }

            var now = DateTime.UtcNow;
            var notifications = enrolledStudents.Select(studentId => new Notification
            {
                IdNguoiDung = studentId,
                TieuDe = tieuDe,
                NoiDung = noiDung,
                Loai = loai,
                TrangThai = "Chưa đọc",
                IdKhoaHoc = courseId,
                NgayTao = now
            }).ToList();

            await _dbSet.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
        }
    }
}








