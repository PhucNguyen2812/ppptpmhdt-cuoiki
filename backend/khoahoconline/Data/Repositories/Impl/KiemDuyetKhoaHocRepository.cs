using Microsoft.EntityFrameworkCore;
using khoahoconline.Data.Entities;
using khoahoconline.Dtos;

namespace khoahoconline.Data.Repositories.Impl
{
    public class KiemDuyetKhoaHocRepository : BaseRepository<KiemDuyetKhoaHoc>, IKiemDuyetKhoaHocRepository
    {
        public KiemDuyetKhoaHocRepository(CourseOnlDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<KiemDuyetKhoaHoc>> GetPagedAsync(string? trangThai = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(k => k.IdKhoaHocNavigation)
                    .ThenInclude(k => k!.IdGiangVienNavigation)
                .Include(k => k.IdNguoiGuiNavigation)
                .Include(k => k.IdNguoiDuyetNavigation)
                .AsQueryable();

            // Filter by status
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(k => k.TrangThai == trangThai);
            }

            // Order by NgayGui descending
            query = query.OrderByDescending(k => k.NgayGui);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<KiemDuyetKhoaHoc>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<KiemDuyetKhoaHoc?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(k => k.IdKhoaHocNavigation)
                    .ThenInclude(k => k!.IdGiangVienNavigation)
                .Include(k => k.IdKhoaHocNavigation)
                    .ThenInclude(k => k!.IdDanhMucNavigation)
                .Include(k => k.IdKhoaHocNavigation)
                    .ThenInclude(k => k!.Chuongs)
                        .ThenInclude(c => c.BaiGiangs)
                .Include(k => k.IdNguoiGuiNavigation)
                .Include(k => k.IdNguoiDuyetNavigation)
                .FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<KiemDuyetKhoaHoc?> GetByKhoaHocIdAsync(int khoaHocId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(k => k.IdKhoaHocNavigation)
                .Include(k => k.IdNguoiGuiNavigation)
                .Include(k => k.IdNguoiDuyetNavigation)
                .OrderByDescending(k => k.NgayGui)
                .FirstOrDefaultAsync(k => k.IdKhoaHoc == khoaHocId);
        }

        public async Task<bool> HasPendingRequestAsync(int khoaHocId)
        {
            return await _dbSet
                .AnyAsync(k => k.IdKhoaHoc == khoaHocId && k.TrangThai == "Chờ duyệt");
        }
    }
}








