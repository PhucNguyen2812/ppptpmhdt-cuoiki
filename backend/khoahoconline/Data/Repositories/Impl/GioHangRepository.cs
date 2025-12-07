using Microsoft.EntityFrameworkCore;
using khoahoconline.Data.Entities;

namespace khoahoconline.Data.Repositories.Impl
{
    public class GioHangRepository : BaseRepository<GioHang>, IGioHangRepository
    {
        public GioHangRepository(CourseOnlDbContext context) : base(context)
        {
        }

        public async Task<GioHang?> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(g => g.ChiTietGioHangs)
                    .ThenInclude(c => c.IdKhoaHocNavigation)
                        .ThenInclude(k => k.IdGiangVienNavigation)
                .Include(g => g.ChiTietGioHangs)
                    .ThenInclude(c => c.IdKhoaHocNavigation)
                        .ThenInclude(k => k.IdDanhMucNavigation)
                .FirstOrDefaultAsync(g => g.IdNguoiDung == userId);
        }

        public async Task<GioHang> CreateCartForUserAsync(int userId)
        {
            var cart = new GioHang
            {
                IdNguoiDung = userId,
                TongTienGoc = 0,
                TongTienThanhToan = 0,
                SoLuongKhoaHoc = 0,
                NgayTao = DateTime.UtcNow,
                NgayCapNhat = DateTime.UtcNow
            };

            return await CreateAsync(cart);
        }

        public async Task<ChiTietGioHang?> GetCartItemByIdAsync(int id)
        {
            return await _context.ChiTietGioHangs
                .Include(c => c.IdKhoaHocNavigation)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ChiTietGioHang?> GetCartItemByCartAndCourseAsync(int cartId, int courseId)
        {
            return await _context.ChiTietGioHangs
                .Include(c => c.IdKhoaHocNavigation)
                .FirstOrDefaultAsync(c => c.IdGioHang == cartId && c.IdKhoaHoc == courseId);
        }

        public async Task<ChiTietGioHang> AddItemAsync(ChiTietGioHang item)
        {
            await _context.ChiTietGioHangs.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task RemoveItemAsync(ChiTietGioHang item)
        {
            _context.ChiTietGioHangs.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task ClearCartAsync(int cartId)
        {
            var items = await _context.ChiTietGioHangs
                .Where(c => c.IdGioHang == cartId)
                .ToListAsync();

            _context.ChiTietGioHangs.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartTotalAsync(int cartId)
        {
            var cart = await _dbSet
                .Include(g => g.ChiTietGioHangs)
                    .ThenInclude(c => c.IdKhoaHocNavigation)
                .FirstOrDefaultAsync(g => g.Id == cartId);

            if (cart == null) return;

            // Tính tổng tiền gốc
            cart.TongTienGoc = cart.ChiTietGioHangs
                .Where(c => c.IdKhoaHocNavigation != null && c.IdKhoaHocNavigation.GiaBan.HasValue)
                .Sum(c => c.IdKhoaHocNavigation!.GiaBan!.Value);

            // Đếm số lượng khóa học
            cart.SoLuongKhoaHoc = cart.ChiTietGioHangs.Count;

            // Tính tổng tiền thanh toán (chưa có voucher)
            cart.TongTienThanhToan = cart.TongTienGoc - (cart.TienGiamVoucher ?? 0);
            cart.NgayCapNhat = DateTime.UtcNow;

            await UpdateAsync(cart);
        }
    }
}
















