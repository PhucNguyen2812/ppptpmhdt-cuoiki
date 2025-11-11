using khoahoconline.Data.Entities;
using khoahoconline.Dtos;
using khoahoconline.Dtos.NguoiDung;

namespace khoahoconline.Services
{
    public interface INguoiDungService
    {
        Task<NguoiDungDto> createAsync(CreateNguoiDungDto dto);
        Task updateAsync(int id, UpdateNguoiDungDto dto);
        Task<NguoiDungDto?> getByIdAsync(int id);
        Task<PagedResult<NguoiDungDto>> GetAllAsync(int pageNumber, int pageSize, bool active, string? searchTerm = null);
        Task SoftDeleteAsync(int id);
    }
}
