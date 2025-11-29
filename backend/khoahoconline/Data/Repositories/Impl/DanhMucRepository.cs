using Microsoft.EntityFrameworkCore;
using khoahoconline.Data.Entities;

namespace khoahoconline.Data.Repositories.Impl
{
    public class DanhMucRepository : BaseRepository<DanhMucKhoaHoc>, IDanhMucRepository
    {
        public DanhMucRepository(CourseOnlDbContext context) : base(context)
        {
        }

        public async Task<List<DanhMucKhoaHoc>> GetAllActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(dm => dm.TrangThai == true)
                .OrderBy(dm => dm.TenDanhMuc)
                .ToListAsync();
        }

        public async Task<List<DanhMucKhoaHoc>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(dm => dm.TenDanhMuc)
                .ToListAsync();
        }

        public async Task<DanhMucKhoaHoc?> GetByIdAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(dm => dm.Id == id);
        }

        public async Task<DanhMucKhoaHoc?> GetByIdWithCoursesAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(dm => dm.KhoaHocs)
                .FirstOrDefaultAsync(dm => dm.Id == id);
        }

        public async Task<bool> IsTenDanhMucExistsAsync(string tenDanhMuc, int? excludeId = null)
        {
            var query = _dbSet.AsNoTracking()
                .Where(dm => dm.TenDanhMuc.ToLower() == tenDanhMuc.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(dm => dm.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<int> CountCoursesAsync(int danhMucId)
        {
            return await _context.KhoaHocs
                .AsNoTracking()
                .Where(kh => kh.IdDanhMuc == danhMucId && kh.TrangThai == true)
                .CountAsync();
        }

        public async Task<List<DanhMucKhoaHoc>> SearchAsync(string searchTerm)
        {
            searchTerm = searchTerm.Trim().ToLower();
            
            return await _dbSet
                .AsNoTracking()
                .Where(dm => dm.TenDanhMuc.ToLower().Contains(searchTerm) 
                    || (dm.MoTa != null && dm.MoTa.ToLower().Contains(searchTerm)))
                .OrderBy(dm => dm.TenDanhMuc)
                .ToListAsync();
        }

        public Task SoftDeleteAsync(DanhMucKhoaHoc entity)
        {
            entity.TrangThai = false;
            entity.NgayCapNhat = DateTime.Now;
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
    }
}