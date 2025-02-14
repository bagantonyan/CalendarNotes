using CalendarNotes.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CalendarNotes.DAL.Repositories.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        IQueryable<TEntity> GetAll(bool trackChanges);
        IQueryable<TEntity> GetByCondition(Expression<Func<TEntity, bool>> expression, bool trackChanges);
        Task CreateAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
