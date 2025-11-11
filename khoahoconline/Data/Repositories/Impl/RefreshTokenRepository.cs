using Microsoft.EntityFrameworkCore;
using khoahoconline.Data.Entities;

namespace khoahoconline.Data.Repositories.Impl
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(CourseOnlDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            var refreshToken = await _dbSet
                .Include(rt => rt.IdNguoiDungNavigation)
                    .ThenInclude(nd => nd!.NguoiDungVaiTros)
                        .ThenInclude(ndvt => ndvt.IdVaiTroNavigation)
                .FirstOrDefaultAsync(rt => rt.Token == token);
            return refreshToken;
        }
    }
}