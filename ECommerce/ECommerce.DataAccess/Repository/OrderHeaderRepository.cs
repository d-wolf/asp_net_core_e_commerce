using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;

namespace ECommerce.DataAccess.Repository
{
    public class OrderHeaderRepository(ApplicationDbContext context) : Repository<OrderHeader>(context), IOrderHeaderRepository
    {
        public void Update(OrderHeader orderHeader)
        {
            _context.Update(orderHeader);
        }
    }
}