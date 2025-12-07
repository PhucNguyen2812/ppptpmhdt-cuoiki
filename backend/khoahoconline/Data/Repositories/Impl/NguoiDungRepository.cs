using Microsoft.EntityFrameworkCore;
using khoahoconline.Data.Entities;
using khoahoconline.Dtos;
using System.Linq;

namespace khoahoconline.Data.Repositories.Impl
{
    public class NguoiDungRepository : BaseRepository<NguoiDung>, INguoiDungRepository
    {
        public NguoiDungRepository(CourseOnlDbContext context) : base(context)
        {

        }
        public async Task<NguoiDung?> GetByEmailAsync(string email)
        {
            return await _dbSet.AsNoTracking()
                .Include(nguoiDung => nguoiDung.NguoiDungVaiTros)
                    .ThenInclude(ndvt => ndvt.IdVaiTroNavigation)
                .FirstOrDefaultAsync(nguoiDung => nguoiDung.Email == email);
        }

        public async Task<PagedResult<NguoiDung>> GetPagedListAsync(int pageNumber, int pageSize, bool active, string? searchTerm = null)
        {
            IQueryable<NguoiDung> query = _dbSet.AsQueryable();

            query = query.Where(nguoiDung => nguoiDung.TrangThai == active);

            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(nguoiDung =>
                    nguoiDung.HoTen!.ToLower().Contains(searchTerm) ||
                    nguoiDung.Email!.ToLower().Contains(searchTerm));
            }

            var totalAmount = await query.CountAsync();

            var items = await query.AsNoTracking()
                .Include(nguoiDung => nguoiDung.NguoiDungVaiTros)
                    .ThenInclude(ndvt => ndvt.IdVaiTroNavigation)
                .OrderBy(nguoiDung => nguoiDung.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<NguoiDung>
            {
                Items = items,
                TotalCount = totalAmount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public Task SoftDelete(NguoiDung entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public async Task<NguoiDung?> GetByIdWithRolesAsync(int id)
        {
            return await _dbSet.AsNoTracking()
                .Include(nd => nd.NguoiDungVaiTros)
                    .ThenInclude(ndvt => ndvt.IdVaiTroNavigation)
                .FirstOrDefaultAsync(nd => nd.Id == id);
        }

        public async Task<bool> HasRoleAsync(int userId, string roleName)
        {
            return await _context.NguoiDungVaiTros
                .AnyAsync(ndvt => ndvt.IdNguoiDung == userId 
                    && ndvt.IdVaiTroNavigation!.TenVaiTro == roleName);
        }

        public async Task AddRoleToUserAsync(int userId, int roleId)
        {
            var exists = await _context.NguoiDungVaiTros
                .AnyAsync(ndvt => ndvt.IdNguoiDung == userId && ndvt.IdVaiTro == roleId);

            if (!exists)
            {
                var nguoiDungVaiTro = new NguoiDungVaiTro
                {
                    IdNguoiDung = userId,
                    IdVaiTro = roleId
                };
                await _context.NguoiDungVaiTros.AddAsync(nguoiDungVaiTro);
            }
        }

        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            try
            {
                var userRoles = await _context.NguoiDungVaiTros
                    .Where(ndvt => ndvt.IdNguoiDung == userId)
                    .Select(ndvt => ndvt.IdVaiTro)
                    .ToListAsync();

                if (userRoles == null || !userRoles.Any())
                {
                    return new List<string>();
                }

                var roles = await _context.VaiTros
                    .Where(vt => userRoles.Contains(vt.Id))
                    .Select(vt => vt.TenVaiTro)
                    .ToListAsync();

                return roles ?? new List<string>();
            }
            catch (Exception)
            {
                // Return empty list if there's an error
                return new List<string>();
            }
        }

        public async Task RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var userRole = await _context.NguoiDungVaiTros
                .FirstOrDefaultAsync(ndvt => ndvt.IdNguoiDung == userId && ndvt.IdVaiTro == roleId);

            if (userRole != null)
            {
                _context.NguoiDungVaiTros.Remove(userRole);
            }
        }

        public async Task RemoveAllRolesFromUserAsync(int userId)
        {
            var userRoles = await _context.NguoiDungVaiTros
                .Where(ndvt => ndvt.IdNguoiDung == userId)
                .ToListAsync();

            _context.NguoiDungVaiTros.RemoveRange(userRoles);
        }
        
    }
}