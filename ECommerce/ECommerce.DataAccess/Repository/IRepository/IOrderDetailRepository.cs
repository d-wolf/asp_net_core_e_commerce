using ECommerce.Models.Models;

namespace ECommerce.DataAccess.Repository.IRepository;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    void Update(OrderDetail orderDetail);
}