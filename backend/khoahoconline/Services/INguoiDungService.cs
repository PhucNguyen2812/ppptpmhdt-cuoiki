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
        Task<NguoiDungDetailDto?> GetDetailByIdAsync(int id);
        Task<PagedResult<NguoiDungDto>> GetAllAsync(int pageNumber, int pageSize, bool active, string? searchTerm = null);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task<bool> RegisterAsInstructorAsync(int userId);
        Task UpdateProfileAsync(int id, UpdateProfileDto dto);
        Task ChangePasswordAsync(int id, ChangePasswordDto dto);
        Task<string> UploadAvatarAsync(int id, IFormFile file);
    }
}