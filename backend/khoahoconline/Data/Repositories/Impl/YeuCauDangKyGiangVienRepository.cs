using Microsoft.EntityFrameworkCore;
using khoahoconline.Data.Entities;
using khoahoconline.Dtos;

namespace khoahoconline.Data.Repositories.Impl
{
    public class YeuCauDangKyGiangVienRepository : BaseRepository<YeuCauDangKyGiangVien>, IYeuCauDangKyGiangVienRepository
    {
        public YeuCauDangKyGiangVienRepository(CourseOnlDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<YeuCauDangKyGiangVien>> GetPagedAsync(string? trangThai = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(y => y.IdHocVienNavigation)
                .Include(y => y.IdNguoiDuyetNavigation)
                .AsQueryable();

            // Filter by status
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(y => y.TrangThai == trangThai);
            }

            // Order by NgayGui descending
            query = query.OrderByDescending(y => y.NgayGui);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<YeuCauDangKyGiangVien>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<YeuCauDangKyGiangVien?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(y => y.IdHocVienNavigation)
                .Include(y => y.IdNguoiDuyetNavigation)
                .FirstOrDefaultAsync(y => y.Id == id);
        }

        public async Task<YeuCauDangKyGiangVien?> GetByHocVienIdAsync(int hocVienId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(y => y.IdHocVienNavigation)
                .Include(y => y.IdNguoiDuyetNavigation)
                .OrderByDescending(y => y.NgayGui)
                .FirstOrDefaultAsync(y => y.IdHocVien == hocVienId);
        }

        public async Task<bool> HasPendingRequestAsync(int hocVienId)
        {
            return await _dbSet
                .AnyAsync(y => y.IdHocVien == hocVienId && y.TrangThai == "Chờ duyệt");
        }
    }
}








