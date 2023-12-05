using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;

namespace ECommerce.DataAccess.Repository
{
    public class ProductImageRepository(ApplicationDbContext context) : Repository<ProductImage>(context), IProductImageRepository
    {
        public void Update(ProductImage productImage)
        {
            _context.Update(productImage);
        }
    }
}