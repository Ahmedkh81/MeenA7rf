using DataAccess.Data;
using DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _db;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task<bool> CreateAsync(T entity)
        {
            try
            {
                await _db.AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex:{ex}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                _db.Update(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex:{ex}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            try
            {
                _db.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex:{ex}");
                return false;
            }
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? include = null, bool tracked = true)
        {
            var entities = _db.AsQueryable();

            if (expression != null)
                entities = entities.Where(expression);

            if (include != null)
            {
                foreach(var inc in include)
                    entities = entities.Include(inc);
            }

            if (!tracked)
                entities = entities.AsNoTracking();

            return await (entities.ToListAsync());
        }

        public async Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? include = null, bool tracked = true)
        {
            return (await GetAsync(expression, include, tracked)).FirstOrDefault();
        }

        public async Task<bool> CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex:{ex}");
                return false;
            }
        }
    }
}
