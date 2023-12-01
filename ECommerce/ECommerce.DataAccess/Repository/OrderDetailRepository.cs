using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;

namespace ECommerce.DataAccess.Repository
{
    public class OrderDetailRepository(ApplicationDbContext context) : Repository<OrderDetail>(context), IOrderDetailRepository
    {
        public void Update(OrderDetail orderDetail)
        {
            _context.Update(orderDetail);
        }
    }
}