using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SchoolUni.Database.Data
{
    public interface IGenericRepo<TEntity> where TEntity : class
    {
        void Delete(object id);
        void Delete(TEntity entityToDelete);
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");
        TEntity GetByID(object id);
        Task<TEntity> GetByIDAsync(object id);

        void Insert(TEntity entity);
        void InsertAsync(TEntity entity);
        void Update(TEntity entityToUpdate);
    }
}