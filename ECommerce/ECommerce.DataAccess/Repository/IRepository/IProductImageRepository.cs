using ECommerce.Models.Models;

namespace ECommerce.DataAccess.Repository.IRepository;

public interface IProductImageRepository : IRepository<ProductImage>
{
    void Update(ProductImage productImage);
}