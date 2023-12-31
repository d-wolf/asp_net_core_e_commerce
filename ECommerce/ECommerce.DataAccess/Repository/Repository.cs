
using System.Linq.Expressions;
using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DataAccess.Repository
{
    public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context = context;

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, params string[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var item in includeProperties)
            {
                query = query.Include(item);
            }


            return query.AsNoTracking().Where(filter ?? (_ => true));
        }

        public T? GetFirstOrDefault(Expression<Func<T, bool>> filter, params string[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var item in includeProperties)
            {
                query = query.Include(item);
            }
            return query
                    .AsNoTracking()
                    .FirstOrDefault(filter);
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }
    }
}