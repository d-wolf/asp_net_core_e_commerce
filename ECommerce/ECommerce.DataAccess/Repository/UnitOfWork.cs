using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;

namespace ECommerce.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            
            Category = new CategoryRepository(_context);
            Product = new ProductRepository(_context);
        }

        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}