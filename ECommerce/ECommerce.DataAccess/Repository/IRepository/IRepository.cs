using System.Linq.Expressions;

namespace ECommerce.DataAccess.Repository.IRepository;

public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, params string[] includeProperties);
    T? GetFirstOrDefault(Expression<Func<T, bool>> filter, params string[] includeProperties);
    void Add(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}