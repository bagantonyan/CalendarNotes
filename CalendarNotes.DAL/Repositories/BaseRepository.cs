using CalendarNotes.DAL.Contexts;
using CalendarNotes.DAL.Entities;
using CalendarNotes.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CalendarNotes.DAL.Repositories
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(CalendarNotesDbContext dbContext) => _dbSet = dbContext.Set<TEntity>();

        public IQueryable<TEntity> GetAll(bool trackChanges)
            => trackChanges ?
            _dbSet :
            _dbSet.AsNoTracking();

        public IQueryable<TEntity> GetByCondition(Expression<Func<TEntity, bool>> expression, bool trackChanges)
            => trackChanges ?
            _dbSet.Where(expression) :
            _dbSet.Where(expression).AsNoTracking();

        public async Task CreateAsync(TEntity entity)
            => await _dbSet.AddAsync(entity);

        public void Update(TEntity entity)
            => _dbSet.Update(entity);

        public void Delete(TEntity entity)
            => _dbSet.Remove(entity);
    }
}