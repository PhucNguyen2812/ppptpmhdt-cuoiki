
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
    }
}
