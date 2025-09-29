using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<bool> CreateAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? include = null, bool tracked = true);
        Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? include = null, bool tracked = true);
        Task<bool> CommitAsync();
    }
}
