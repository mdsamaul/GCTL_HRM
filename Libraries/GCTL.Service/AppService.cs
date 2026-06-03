using GCTL.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service
{
    public class AppService<T> where T : class
    {
        private readonly IRepository<T> repository;

        public AppService(IRepository<T> repository)
        {
            this.repository = repository;
        }

        // For next code 

        public async Task<string> GenerateNextCode(Expression<Func<T, string>> codeSelector, int codeLength)
        {
            var items = await repository.GetAllAsync();
            var lastCode = items.Max(codeSelector.Compile());
            int nextCode = 1;

            if (!string.IsNullOrEmpty(lastCode))
            {
                int lastNumber = int.Parse(lastCode.TrimStart('0'));
                lastNumber++;
                nextCode = lastNumber;
            }

            return nextCode.ToString($"D{codeLength}");
        }

        //
        public List<T> GetAll()
        {
            return repository.GetAll().ToList();
        }
        public T GetByCode(string code)
        {
            return repository.GetById(code);
        }

        public T Add(T entity)
        {
            return repository.Add(entity);
        }

        public void Update(T entity)
        {
            repository.Update(entity);
        }

        public bool Delete(string code)
        {
            var entity = GetByCode(code);
            if (entity != null)
            {
                Delete(entity);
                return true;
            }
            return false;
        }

        public void Delete(T entity)
        {
            repository.Delete(entity);
        }
        //
        public async Task<T> AddAsync(T entity)
        {
            await repository.AddAsync(entity);
            return entity; // Return the added entity or you may return a result indicating success.
        }

        public async Task UpdateAsync(T entity)
        {
            await repository.UpdateAsync(entity);
        }
        public async Task<T> GetByCodeAsync(string code)
        {
            return await repository.GetByIdAsync(code);
        }

      

        public async Task<bool> DeleteAsync(string code)
        {
            var entity = await GetByCodeAsync(code);
            if (entity != null)
            {
              await  DeleteAsync(entity);
                return true;
            }
            return false;
        }
       
        public async Task DeleteAsync(T entity)
        {
            await repository.DeleteAsync(entity);
        }

    }
}