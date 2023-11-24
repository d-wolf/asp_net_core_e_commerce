using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;

namespace ECommerce.DataAccess.Repository
{
    public class CategoryRepository(ApplicationDbContext context) : Repository<Category>(context), ICategoryRepository
    {
        public void Update(Category category)
        {
            _context.Update(category);
        }
    }
}