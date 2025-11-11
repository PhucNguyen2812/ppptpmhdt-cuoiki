
using Microsoft.EntityFrameworkCore;

namespace khoahoconline.Data.Repositories.Impl
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly CourseOnlDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(CourseOnlDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }


        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
    }
}
