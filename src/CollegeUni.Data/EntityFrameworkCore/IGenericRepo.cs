using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CollegeUni.Data.EntityFrameworkCore
{
    public interface IGenericRepo<TEntity> where TEntity : class
    {
        void Delete(object id);
        void Delete(TEntity entityToDelete);
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");
        TEntity GetById(object id);
        Task<TEntity> GetByIdAsync(object id);
        TEntity GetById(object id, string includeProperties);
        Task<TEntity> GetByIdAsync(object id, string includeProperties);
        void Insert(TEntity entity);
        void InsertAsync(TEntity entity);
        void Update(TEntity entityToUpdate);
    }
}