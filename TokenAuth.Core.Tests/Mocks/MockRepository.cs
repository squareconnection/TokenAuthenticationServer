using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using TokenAuth.Contracts;
using TokenAuth.Models;

namespace TokenAuth.Core.Tests.Mocks
{
    public class MockRepository<TEntity> : IAsyncRepository<TEntity> where TEntity : EntityBase
    {
        internal List<TEntity> _local;

        public MockRepository()
        {
            _local = new List<TEntity>();
        }

        public Task<int> CommitAsync()
        {
			return Task<int>.Factory.StartNew(() => 1);
        }

        public void Delete(TEntity entity)
        {
			var existing = FindAsync(entity.Id).Result;
			_local.Remove(existing);
        }

        public void Delete(object id)
        {
			var existing = FindAsync(id).Result;
			_local.Remove(existing);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> FindAsync(object id)
        {
            return await Task.Run(()=>_local.Find(u => u.Id == id.ToString()));
        }

        public async Task<TEntity> FirstOrDefaultAsync(Func<TEntity, bool> filter)
        {
            return await Task.Run(() => _local.FirstOrDefault(filter));
        }

        public void Insert(TEntity entity)
        {
            if (String.IsNullOrEmpty(entity.Id))
                entity.Id = Guid.NewGuid().ToString();
            
            _local.Add(entity);
        }


        public Task<List<string>> ModelMetaAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<TEntity>> ToListAsync()
        {
            return await Task.Run(()=>_local.ToList());
        }

        public void Update(TEntity entity)
        {
            var existing = FindAsync(entity.Id).Result;
            _local.Remove(existing);
            _local.Add(entity);

        }
    }
}
