using System.Linq.Expressions;
using ECommerce.Models.Models;

namespace ECommerce.DataAccess.Repository.IRepository;

public interface IProductRepository : IRepository<Product>
{
    void Update(Product product);
}