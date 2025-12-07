using Microsoft.EntityFrameworkCore;
using khoahoconline.Data.Entities;

namespace khoahoconline.Data.Repositories.Impl
{
    public class DanhGiaRepository : BaseRepository<DanhGiaKhoaHoc>, IDanhGiaRepository
    {
        public DanhGiaRepository(CourseOnlDbContext context) : base(context)
        {
        }

        public async Task<DanhGiaKhoaHoc?> GetByCourseAndStudentAsync(int idKhoaHoc, int idHocVien)
        {
            return await _dbSet
                .Include(d => d.IdHocVienNavigation)
                .FirstOrDefaultAsync(d => d.IdKhoaHoc == idKhoaHoc && d.IdHocVien == idHocVien && d.TrangThai == true);
        }

        public async Task<List<DanhGiaKhoaHoc>> GetByCourseIdAsync(int idKhoaHoc, int? diemDanhGia = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _dbSet
                .Include(d => d.IdHocVienNavigation)
                .Where(d => d.IdKhoaHoc == idKhoaHoc && d.TrangThai == true);

            if (diemDanhGia.HasValue)
            {
                query = query.Where(d => d.DiemDanhGia == diemDanhGia.Value);
            }

            return await query
                .OrderByDescending(d => d.NgayDanhGia)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountByCourseIdAsync(int idKhoaHoc, int? diemDanhGia = null)
        {
            var query = _dbSet
                .Where(d => d.IdKhoaHoc == idKhoaHoc && d.TrangThai == true);

            if (diemDanhGia.HasValue)
            {
                query = query.Where(d => d.DiemDanhGia == diemDanhGia.Value);
            }

            return await query.CountAsync();
        }

        public async Task<double> GetAverageRatingByCourseIdAsync(int idKhoaHoc)
        {
            var average = await _dbSet
                .Where(d => d.IdKhoaHoc == idKhoaHoc && d.TrangThai == true)
                .AverageAsync(d => (double?)d.DiemDanhGia);

            return average ?? 0.0;
        }

        public async Task<Dictionary<int, int>> GetRatingDistributionByCourseIdAsync(int idKhoaHoc)
        {
            var distribution = await _dbSet
                .Where(d => d.IdKhoaHoc == idKhoaHoc && d.TrangThai == true)
                .GroupBy(d => d.DiemDanhGia)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Rating, x => x.Count);

            // Ensure all ratings 1-5 are present
            for (int i = 1; i <= 5; i++)
            {
                if (!distribution.ContainsKey(i))
                {
                    distribution[i] = 0;
                }
            }

            return distribution;
        }
    }
}




