using CollegeUni.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CollegeUni.Data.EntityFrameworkCore
{
    public class GenericRepo<TEntity> : IGenericRepo<TEntity> where TEntity : BaseEntity
    {
        internal AuthContext context;
        internal DbSet<TEntity> dbSet;

        public GenericRepo(AuthContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }

        public virtual TEntity GetById(object id)
        {
            return dbSet.Find(id);
        }

        public virtual Task<TEntity> GetByIdAsync(object id)
        {
            return dbSet.FindAsync(id);
        }

        public virtual TEntity GetById(object id, string includeProperties)
        {
            return GetByIdAsync(id, includeProperties).GetAwaiter().GetResult();
        }

        public Task<TEntity> GetByIdAsync(object id, string includeProperties)
        {
            return Get(null, null, includeProperties).SingleOrDefaultAsync(e => e.Id.Equals(id));
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void InsertAsync(TEntity entity)
        {
            dbSet.AddAsync(entity);
        }
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}
