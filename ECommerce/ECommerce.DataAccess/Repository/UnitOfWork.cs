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
            Company = new CompanyRepository(_context);
            ShoppingCart = new ShoppingCartRepository(_context);
            ApplicationUser = new ApplicationUserRepository(_context);
            OrderHeaderRepository = new OrderHeaderRepository(_context);
            OrderDetailRepository = new OrderDetailRepository(_context);
        }

        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrderHeaderRepository OrderHeaderRepository { get; private set; }
        public IOrderDetailRepository OrderDetailRepository { get; private set; }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}