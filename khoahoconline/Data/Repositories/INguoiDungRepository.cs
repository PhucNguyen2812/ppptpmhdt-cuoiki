using khoahoconline.Data.Entities;
using khoahoconline.Dtos;
using khoahoconline.Dtos.NguoiDung;

namespace khoahoconline.Data.Repositories
{
    public interface INguoiDungRepository : IBaseRepository<NguoiDung>
    {
        Task<PagedResult<NguoiDung>> GetPagedListAsync(int pageNumber, int pageSize, bool active, string? searchTerm = null);
        Task SoftDelete(NguoiDung entity);
        Task<NguoiDung?> GetByEmailAsync(string username);
    }
}
