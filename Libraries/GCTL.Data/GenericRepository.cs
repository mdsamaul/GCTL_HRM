using GCTL.Core.Data;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

namespace GCTL.Data
{

    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly GCTL_ERP_DB_DatapathContext context;

        public GenericRepository(GCTL_ERP_DB_DatapathContext context)
        {
            this.context = context;
        }

        public IQueryable<T> All()
        {
            return context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return context.Set<T>().ToList();
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> expression)
        {
            return context.Set<T>().Where(expression);
        }

        public IQueryable<T> Get(FormattableString sql)
        {
            return context.Set<T>().FromSqlInterpolated(sql);
        }

        public T GetById(object id)
        {
            return context.Set<T>().Find(id);
        }

        public T Add(T entity)
        {
            context.Set<T>().Add(entity);
            context.SaveChanges();
            return entity;
        }

        public void Add(IEnumerable<T> entities)
        {
            context.Set<T>().AddRange(entities);
            context.SaveChanges();
        }

        public void Update(T entity)
        {
            //    var state = context.Entry(entity).State;


            // context.Entry(entity).State = EntityState.Modified;
            context.Set<T>().Update(entity);
            context.SaveChanges();
        }

        public void Update(IEnumerable<T> entities)
        {
            context.Set<T>().UpdateRange(entities);
            context.SaveChanges();
        }

        public void Delete(object id)
        {
            var entity = GetById(id);
            context.Set<T>().Remove(entity);
            context.SaveChanges();
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
            context.SaveChanges();
        }

        public void Delete(IEnumerable<T> entities)
        {
            context.Set<T>().RemoveRange(entities);
            context.SaveChanges();
        }


        //task async
        public async Task AddAsync(T entity)
        {
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await context.Set<T>().AddRangeAsync(entities);
            await context.SaveChangesAsync();
        }

        public async Task<IQueryable<T>> AllAsync()
        {
            return await Task.FromResult(context.Set<T>());
        }

        public async Task<IQueryable<T>> FindByAsync(Expression<Func<T, bool>> expression)
        {
            return await Task.FromResult(context.Set<T>().Where(expression));
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            context.Set<T>().Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            context.Set<T>().UpdateRange(entities);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(object id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();
            }
        }



        public async Task DeleteAsync(T entity)
        {
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            context.Set<T>().RemoveRange(entities);
            await context.SaveChangesAsync();
        }



        public async Task BeginTransactionAsync()
        {
            await context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            var transaction = context.Database.CurrentTransaction;
            if (transaction != null)
            {
                await transaction.CommitAsync();
                await transaction.DisposeAsync();
            }
        }

        public async Task RollbackTransactionAsync()
        {
            var transaction = context.Database.CurrentTransaction;
            if (transaction != null)
            {
                await transaction.RollbackAsync();
                await transaction.DisposeAsync();
            }
        }
        public async Task DisposeTransactionAsync()
        {
            var transaction = context.Database.CurrentTransaction;
            if (transaction != null)
            {
                await transaction.DisposeAsync();
                await transaction.DisposeAsync();
            }
        }
        public string GetTableName()

        {

            var entityType = context.Model.FindEntityType(typeof(T));

            return entityType?.GetTableName() ?? typeof(T).Name;

        }


    }
}
