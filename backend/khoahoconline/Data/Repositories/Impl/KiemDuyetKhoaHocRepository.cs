using Microsoft.EntityFrameworkCore;
using khoahoconline.Data.Entities;

namespace khoahoconline.Data.Repositories.Impl
{
    public class KiemDuyetKhoaHocRepository : BaseRepository<KiemDuyetKhoaHoc>, IKiemDuyetKhoaHocRepository
    {
        public KiemDuyetKhoaHocRepository(CourseOnlDbContext context) : base(context)
        {
        }

        public async Task<KiemDuyetKhoaHoc?> GetLatestByCourseIdAsync(int courseId)
        {
            return await _dbSet
                .Include(k => k.IdKhoaHocNavigation)
                .Include(k => k.IdNguoiGuiNavigation)
                .Include(k => k.IdNguoiKiemDuyetNavigation)
                .Where(k => k.IdKhoaHoc == courseId)
                .OrderByDescending(k => k.PhienBan)
                .ThenByDescending(k => k.NgayGui)
                .FirstOrDefaultAsync();
        }

        public async Task<KiemDuyetKhoaHoc?> GetByCourseIdAndVersionAsync(int courseId, int version)
        {
            return await _dbSet
                .Include(k => k.IdKhoaHocNavigation)
                .Include(k => k.IdNguoiGuiNavigation)
                .Include(k => k.IdNguoiKiemDuyetNavigation)
                .FirstOrDefaultAsync(k => k.IdKhoaHoc == courseId && k.PhienBan == version);
        }

        public async Task<List<KiemDuyetKhoaHoc>> GetPendingApprovalsAsync()
        {
            return await _dbSet
                .Include(k => k.IdKhoaHocNavigation)
                    .ThenInclude(k => k.IdDanhMucNavigation)
                .Include(k => k.IdKhoaHocNavigation)
                    .ThenInclude(k => k.IdGiangVienNavigation)
                .Include(k => k.IdNguoiGuiNavigation)
                .Where(k => k.TrangThaiKiemDuyet == Helpers.KiemDuyetConstants.ChoKiemDuyet)
                .OrderByDescending(k => k.NgayGui)
                .ToListAsync();
        }

        public async Task<bool> IsCoursePendingAsync(int courseId)
        {
            var latest = await GetLatestByCourseIdAsync(courseId);
            return latest != null && latest.TrangThaiKiemDuyet == Helpers.KiemDuyetConstants.ChoKiemDuyet;
        }

        public async Task<List<KiemDuyetKhoaHoc>> GetAllApprovalsAsync(string? status = null)
        {
            // Lấy tất cả các bản kiểm duyệt
            var allApprovals = await _dbSet
                .Include(k => k.IdKhoaHocNavigation)
                    .ThenInclude(k => k.IdDanhMucNavigation)
                .Include(k => k.IdKhoaHocNavigation)
                    .ThenInclude(k => k.IdGiangVienNavigation)
                .Include(k => k.IdNguoiGuiNavigation)
                .Include(k => k.IdNguoiKiemDuyetNavigation)
                .ToListAsync();

            // Lấy bản mới nhất của mỗi khóa học (theo PhienBan và NgayGui)
            var latestApprovals = allApprovals
                .GroupBy(kd => kd.IdKhoaHoc)
                .Select(g => g.OrderByDescending(kd => kd.PhienBan).ThenByDescending(kd => kd.NgayGui).First())
                .ToList();

            // Filter theo trạng thái nếu có
            if (!string.IsNullOrWhiteSpace(status))
            {
                latestApprovals = latestApprovals
                    .Where(kd => kd.TrangThaiKiemDuyet == status)
                    .ToList();
            }

            // Sắp xếp theo ngày gửi mới nhất
            return latestApprovals
                .OrderByDescending(k => k.NgayGui)
                .ToList();
        }
    }
}


