using Microsoft.EntityFrameworkCore.Storage;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace khoahoconline.Data.Repositories.Impl
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly CourseOnlDbContext _context;
        private IDbContextTransaction? _transaction;
        private NguoiDungRepository? _nguoiDungRepository;
        private RefreshTokenRepository? _refreshTokenRepository;
        private VaiTroRepository? _vaiTroRepository;
        private DanhMucRepository? _danhMucRepository;
        private KhoaHocRepository? _khoaHocRepository;
        private GioHangRepository? _gioHangRepository;
        private NotificationRepository? _notificationRepository;
        private YeuCauDangKyGiangVienRepository? _yeuCauDangKyGiangVienRepository;
        private KiemDuyetKhoaHocRepository? _kiemDuyetKhoaHocRepository;
        private DanhGiaRepository? _danhGiaRepository;

        public UnitOfWork(CourseOnlDbContext context)
        {
            _context = context;
        }

        public IVaiTroRepository VaiTroRepository => 
            _vaiTroRepository ??= new VaiTroRepository(_context);

        public IRefreshTokenRepository RefreshTokenRepository => 
            _refreshTokenRepository ??= new RefreshTokenRepository(_context);

        public INguoiDungRepository NguoiDungRepository =>
            _nguoiDungRepository ??= new NguoiDungRepository(_context);

        public IDanhMucRepository DanhMucRepository =>
            _danhMucRepository ??= new DanhMucRepository(_context);

        public IKhoaHocRepository KhoaHocRepository =>
            _khoaHocRepository ??= new KhoaHocRepository(_context);

        public IGioHangRepository GioHangRepository =>
            _gioHangRepository ??= new GioHangRepository(_context);

        public INotificationRepository NotificationRepository =>
            _notificationRepository ??= new NotificationRepository(_context);

        public IYeuCauDangKyGiangVienRepository YeuCauDangKyGiangVienRepository =>
            _yeuCauDangKyGiangVienRepository ??= new YeuCauDangKyGiangVienRepository(_context);

        public IKiemDuyetKhoaHocRepository KiemDuyetKhoaHocRepository =>
            _kiemDuyetKhoaHocRepository ??= new KiemDuyetKhoaHocRepository(_context);

        public IDanhGiaRepository DanhGiaRepository =>
            _danhGiaRepository ??= new DanhGiaRepository(_context);

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                Dispose();
            }
        }

        public void Dispose()
        {
            _transaction?.DisposeAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                Dispose();
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public Data.CourseOnlDbContext GetDbContext()
        {
            return _context;
        }
    }
}