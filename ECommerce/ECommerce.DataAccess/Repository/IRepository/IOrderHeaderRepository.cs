using ECommerce.Models.Models;

namespace ECommerce.DataAccess.Repository.IRepository;

public interface IOrderHeaderRepository : IRepository<OrderHeader>
{
    void Update(OrderHeader orderHeader);
}