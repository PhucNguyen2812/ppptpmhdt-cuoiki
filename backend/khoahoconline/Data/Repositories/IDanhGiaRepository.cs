using khoahoconline.Data.Entities;

namespace khoahoconline.Data.Repositories
{
    public interface IDanhGiaRepository : IBaseRepository<DanhGiaKhoaHoc>
    {
        Task<DanhGiaKhoaHoc?> GetByCourseAndStudentAsync(int idKhoaHoc, int idHocVien);
        Task<List<DanhGiaKhoaHoc>> GetByCourseIdAsync(int idKhoaHoc, int? diemDanhGia = null, int pageNumber = 1, int pageSize = 10);
        Task<int> GetTotalCountByCourseIdAsync(int idKhoaHoc, int? diemDanhGia = null);
        Task<double> GetAverageRatingByCourseIdAsync(int idKhoaHoc);
        Task<Dictionary<int, int>> GetRatingDistributionByCourseIdAsync(int idKhoaHoc);
    }
}




