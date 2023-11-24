using System.Linq.Expressions;
using ECommerce.Models.Models;

namespace ECommerce.DataAccess.Repository.IRepository;

public interface ICategoryRepository : IRepository<Category>
{
    void Update(Category category);
}