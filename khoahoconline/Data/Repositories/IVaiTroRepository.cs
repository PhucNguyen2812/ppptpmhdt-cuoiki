using khoahoconline.Data.Entities;

namespace khoahoconline.Data.Repositories
{
    public interface IVaiTroRepository : IBaseRepository<VaiTro>
    {
        Task<VaiTro?> GetByTenVaiTroAsync(string tenVaiTro);
        Task SoftDeleteAsync(VaiTro vaiTro);
    }
}
