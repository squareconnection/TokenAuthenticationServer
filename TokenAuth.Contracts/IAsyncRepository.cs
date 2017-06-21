using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TokenAuth.Contracts
{
    public interface IAsyncRepository<T>
    {
		Task<int> CommitAsync();
		void Delete(T entity);
		void Delete(object id);
		void Dispose();

		Task<List<T>> ToListAsync();
		Task<T> FindAsync(object id);
		Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter);
		Task<T> InsertAsync(T entity);
		Task<List<string>> ModelMetaAsync();
		Task<T> UpdateAsync(T entity);
    }
}
