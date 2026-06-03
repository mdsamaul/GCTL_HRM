using GCTL.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service
{
    public class AppServiceTest<T, TKey> where T : class
    {
        private readonly IGenericRepository<T, TKey> _repository;

        public AppServiceTest(IGenericRepository<T, TKey> repository)
        {
            _repository = repository;
        }

        public async Task<List<T>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.ToList();
        }

        public virtual async Task<T?> GetByIdAsync(TKey id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            await _repository.AddAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(TKey id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
                return true;
            }
            return false;
        }

        public async Task DeleteAsync(T entity)
        {
            await _repository.DeleteAsync(entity);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.AnyAsync(predicate);
        }
    }

}
