using Microsoft.EntityFrameworkCore;
using khoahoconline.Data.Entities;
using khoahoconline.Dtos;
using khoahoconline.Dtos.KhoaHoc;

namespace khoahoconline.Data.Repositories.Impl
{
    public class KhoaHocRepository : BaseRepository<KhoaHoc>, IKhoaHocRepository
    {
        public KhoaHocRepository(CourseOnlDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<KhoaHoc>> GetPagedAsync(KhoaHocFilterDto filter)
        {
            // Lấy danh sách ID khóa học đã được duyệt (lấy phiên bản mới nhất của mỗi khóa học)
            var allApprovals = await _context.KiemDuyetKhoaHocs
                .AsNoTracking()
                .ToListAsync();

            var approvedCourseIds = allApprovals
                .GroupBy(kd => kd.IdKhoaHoc)
                .Where(g => 
                {
                    var latest = g.OrderByDescending(kd => kd.PhienBan).ThenByDescending(kd => kd.NgayGui).First();
                    return latest.TrangThaiKiemDuyet == Helpers.KiemDuyetConstants.DaDuyet;
                })
                .Select(g => g.Key)
                .ToList();

            var query = _dbSet
                .AsNoTracking()
                .Include(k => k.IdDanhMucNavigation)
                .Include(k => k.IdGiangVienNavigation)
                .Where(k => k.TrangThai == true && !k.IsDeleted && approvedCourseIds.Contains(k.Id)); // Chỉ lấy khóa học active, đã duyệt

            // Search
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchTerm = filter.Search.Trim().ToLower();
                query = query.Where(k => 
                    k.TenKhoaHoc != null && k.TenKhoaHoc.ToLower().Contains(searchTerm));
            }

            // Filter by category
            if (filter.CategoryId.HasValue)
            {
                query = query.Where(k => k.IdDanhMuc == filter.CategoryId.Value);
            }

            // Filter by minimum rating
            if (filter.MinRating.HasValue)
            {
                query = query.Where(k => k.DiemDanhGia >= filter.MinRating.Value);
            }

            // Filter by level
            if (!string.IsNullOrWhiteSpace(filter.Level))
            {
                query = query.Where(k => k.MucDo == filter.Level);
            }

            // Sorting
            query = filter.SortBy?.ToLower() switch
            {
                "best-selling" => query.OrderByDescending(k => k.SoLuongHocVien),
                "highest-rated" => query.OrderByDescending(k => k.DiemDanhGia)
                                        .ThenByDescending(k => k.SoLuongDanhGia),
                "newest" => query.OrderByDescending(k => k.NgayTao),
                "price-low-high" => query.OrderBy(k => k.GiaBan),
                "price-high-low" => query.OrderByDescending(k => k.GiaBan),
                _ => query.OrderByDescending(k => k.DiemDanhGia) // Default: highest rated
            };

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<KhoaHoc>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<KhoaHoc?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(k => k.IdDanhMucNavigation)
                .Include(k => k.IdGiangVienNavigation)
                .FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<KhoaHoc?> GetByIdWithCurriculumAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(k => k.Chuongs.OrderBy(c => c.ThuTu))
                    .ThenInclude(c => c.BaiGiangs.OrderBy(b => b.ThuTu))
                .FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<List<KhoaHoc>> GetFeaturedCoursesAsync(int take = 8)
        {
            // Lấy danh sách ID khóa học đã được duyệt
            var allApprovals = await _context.KiemDuyetKhoaHocs
                .AsNoTracking()
                .ToListAsync();

            var approvedCourseIds = allApprovals
                .GroupBy(kd => kd.IdKhoaHoc)
                .Where(g => 
                {
                    var latest = g.OrderByDescending(kd => kd.PhienBan).ThenByDescending(kd => kd.NgayGui).First();
                    return latest.TrangThaiKiemDuyet == Helpers.KiemDuyetConstants.DaDuyet;
                })
                .Select(g => g.Key)
                .ToList();

            return await _dbSet
                .AsNoTracking()
                .Include(k => k.IdDanhMucNavigation)
                .Include(k => k.IdGiangVienNavigation)
                .Where(k => k.TrangThai == true 
                    && !k.IsDeleted
                    && approvedCourseIds.Contains(k.Id)
                    && k.DiemDanhGia >= 4.5m 
                    && k.SoLuongHocVien > 0)
                .OrderByDescending(k => k.DiemDanhGia)
                .ThenByDescending(k => k.SoLuongHocVien)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<KhoaHoc>> GetBestSellingCoursesAsync(int take = 8)
        {
            // Lấy danh sách ID khóa học đã được duyệt
            var allApprovals = await _context.KiemDuyetKhoaHocs
                .AsNoTracking()
                .ToListAsync();

            var approvedCourseIds = allApprovals
                .GroupBy(kd => kd.IdKhoaHoc)
                .Where(g => 
                {
                    var latest = g.OrderByDescending(kd => kd.PhienBan).ThenByDescending(kd => kd.NgayGui).First();
                    return latest.TrangThaiKiemDuyet == Helpers.KiemDuyetConstants.DaDuyet;
                })
                .Select(g => g.Key)
                .ToList();

            return await _dbSet
                .AsNoTracking()
                .Include(k => k.IdDanhMucNavigation)
                .Include(k => k.IdGiangVienNavigation)
                .Where(k => k.TrangThai == true && !k.IsDeleted && approvedCourseIds.Contains(k.Id))
                .OrderByDescending(k => k.SoLuongHocVien)
                .ThenByDescending(k => k.DiemDanhGia)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<KhoaHoc>> GetNewestCoursesAsync(int take = 8)
        {
            // Lấy danh sách ID khóa học đã được duyệt
            var allApprovals = await _context.KiemDuyetKhoaHocs
                .AsNoTracking()
                .ToListAsync();

            var approvedCourseIds = allApprovals
                .GroupBy(kd => kd.IdKhoaHoc)
                .Where(g => 
                {
                    var latest = g.OrderByDescending(kd => kd.PhienBan).ThenByDescending(kd => kd.NgayGui).First();
                    return latest.TrangThaiKiemDuyet == Helpers.KiemDuyetConstants.DaDuyet;
                })
                .Select(g => g.Key)
                .ToList();

            return await _dbSet
                .AsNoTracking()
                .Include(k => k.IdDanhMucNavigation)
                .Include(k => k.IdGiangVienNavigation)
                .Where(k => k.TrangThai == true && !k.IsDeleted && approvedCourseIds.Contains(k.Id))
                .OrderByDescending(k => k.NgayTao)
                .Take(take)
                .ToListAsync();
        }

        public async Task<PagedResult<KhoaHoc>> GetByInstructorAsync(int instructorId, int pageNumber = 1, int pageSize = 12)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(k => k.IdDanhMucNavigation)
                .Where(k => k.IdGiangVien == instructorId && !k.IsDeleted) // Giảng viên thấy tất cả khóa học của mình (kể cả chưa duyệt)
                .OrderByDescending(k => k.NgayTao);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<KhoaHoc>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<int> CountByInstructorAsync(int instructorId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(k => k.IdGiangVien == instructorId && k.TrangThai == true)
                .CountAsync();
        }

        public async Task<int> GetTotalDurationAsync(int courseId)
        {
            var totalSeconds = await _context.BaiGiangs
                .AsNoTracking()
                .Where(b => b.IdChuongNavigation!.IdKhoaHoc == courseId 
                    && b.TrangThai == true)
                .SumAsync(b => b.ThoiLuong ?? 0);

            return totalSeconds / 60; // Convert to minutes
        }

        public async Task<int> CountChaptersAsync(int courseId)
        {
            return await _context.Chuongs
                .AsNoTracking()
                .Where(c => c.IdKhoaHoc == courseId && c.TrangThai == true)
                .CountAsync();
        }

        public async Task<int> CountLecturesAsync(int courseId)
        {
            return await _context.BaiGiangs
                .AsNoTracking()
                .Where(b => b.IdChuongNavigation!.IdKhoaHoc == courseId 
                    && b.TrangThai == true)
                .CountAsync();
        }
    }
}